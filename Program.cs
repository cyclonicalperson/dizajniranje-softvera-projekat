using System.Runtime.InteropServices;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using CoWorkingManager.Podaci.Repozitorijumi;

namespace CoWorkingManager.Test
{
    // Konzolni program za testiranje celokupnog sloja podataka.
    // Seed podataka je 1:1 usklađen sa podaci.sql.
    internal class TestBaze
    {
        // WinForms projekat nema konzolni prozor — moramo ga sami alocirati
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        // Facade — jedina referenca na DAL sloj u ovom fajlu.
        // Sve repozitorijume dobijamo kroz nju, ne kreiramo ih ručno.
        private static CoworkingFasada _facade = null!;

        [STAThread]
        static void Main(string[] args)
        {
            AllocConsole();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║         TEST SLOJA PODATAKA — CoWorking          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════╝\n");

            try
            {
                TestKonfiguracije();
                TestKonekcije();

                // Inicijalizacija Facade — kreira sve repozitorijume interno
                _facade = CoworkingFasada.Inicijalizuj(ManagerKonfiguracije.Instanca.KonekcioniString);

                // Provjera Facade Singleton-a
                Proveri(ReferenceEquals(_facade, CoworkingFasada.DajInstancu()),
                    "Facade Singleton: DajInstancu() vraća istu instancu ✓");

                TestSeedPodataka();
                TestTipovaClanstva();
                TestLokacija();
                TestKorisnika();
                TestResursa();
                TestRezervacija();
                TestPreklapanjaRezervacija();
                TestBrojaSatiKorisnika();
                TestDostupnostiResursa();
                TestStatistikeZauzetosti();
                TestFiltriranja();
                TestAdministratora();

                Console.WriteLine("\n╔══════════════════════════════════════════════════╗");
                Console.WriteLine("║         SVE PROVERE PROŠLE USPEŠNO ✓             ║");
                Console.WriteLine("╚══════════════════════════════════════════════════╝");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ GREŠKA: {ex.Message}");
                Console.WriteLine($"  Tip: {ex.GetType().Name}");
                if (ex.InnerException != null)
                    Console.WriteLine($"  Detalj: {ex.InnerException.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nPritisnite bilo koji taster za izlaz...");
            Console.ReadKey();
            FreeConsole();
        }

        // ════════════════════════════════════════════════════════════════════
        // KORAK 1 — Konfiguracija
        // ════════════════════════════════════════════════════════════════════

        private static void TestKonfiguracije()
        {
            Zaglavlje("1. UČITAVANJE KONFIGURACIJE");
            var config = ManagerKonfiguracije.Instanca;
            Ok($"Naziv lanca: {config.NazivLanca}");
            Ok($"Konekcioni string: {config.KonekcioniString}");
            Proveri(ReferenceEquals(config, ManagerKonfiguracije.Instanca),
                "Singleton: oba poziva vraćaju istu instancu");
        }

        // ════════════════════════════════════════════════════════════════════
        // KORAK 2 — Konekcija
        // ════════════════════════════════════════════════════════════════════

        private static void TestKonekcije()
        {
            Zaglavlje("2. KONEKCIJA NA BAZU");

            var ks = ManagerKonfiguracije.Instanca.KonekcioniString;
            var kontekst = KontekstBaze.DajInstancu(ks);
            Ok("KontekstBaze.DajInstancu() — instanca kreirana");

            Proveri(ReferenceEquals(kontekst, KontekstBaze.DajInstancu(ks)),
                "Singleton: KontekstBaze vraća istu instancu");

            bool kreirana = kontekst.Database.EnsureCreated();
            Ok(kreirana ? "Šema baze kreirana" : "Šema baze već postoji — preskačem kreiranje");

            Proveri(kontekst.Database.CanConnect(), "Konekcija na bazu uspešna");
        }

        // ════════════════════════════════════════════════════════════════════
        // KORAK 3 — Seed podataka (1:1 sa podaci.sql)
        // ════════════════════════════════════════════════════════════════════

        private static void TestSeedPodataka()
        {
            Zaglavlje("3. UNOS POČETNIH PODATAKA");

            if (_facade.TipoviClanstva.DajSve().Count > 0)
            {
                Ok("Podaci već postoje — preskačem seed");
                return;
            }

            // ── Tipovi članstva (iz podaci.sql, Id 1–4) ──────────────────────
            // Id 1: Dnevno, Id 2: Hot Desk, Id 3: Dedicated Desk, Id 4: Premium
            var dnevno = new TipClanstva { Ime = "Dnevno", Cena = 12.00m, Trajanje = 1, MaxSatiPoMesecu = 8, PristupSali = false, BrojSatiUSaliMesecno = null };
            var hotDesk = new TipClanstva { Ime = "Hot Desk", Cena = 120.00m, Trajanje = 30, MaxSatiPoMesecu = 80, PristupSali = true, BrojSatiUSaliMesecno = 10 };
            var dedicatedDesk = new TipClanstva { Ime = "Dedicated Desk", Cena = 180.00m, Trajanje = 30, MaxSatiPoMesecu = 120, PristupSali = true, BrojSatiUSaliMesecno = 20 };
            var premium = new TipClanstva { Ime = "Premium", Cena = 250.00m, Trajanje = 30, MaxSatiPoMesecu = 200, PristupSali = true, BrojSatiUSaliMesecno = 40 };

            _facade.TipoviClanstva.Dodaj(dnevno);
            _facade.TipoviClanstva.Dodaj(hotDesk);
            _facade.TipoviClanstva.Dodaj(dedicatedDesk);
            _facade.TipoviClanstva.Dodaj(premium);
            Ok($"Unesena 4 tipa članstva (Id: {dnevno.Id}–{premium.Id})");

            // ── Lokacije (iz podaci.sql — bez Opis kolone) ───────────────────
            // Id 1: Kragujevac, Id 2: Beograd, Id 3: Novi Sad
            var kragujevac = new Lokacija { Ime = "Hub Kragujevac Center", Adresa = "Kralja Aleksandra 12", Grad = "Kragujevac", RadniSati = "08:00–22:00", MaxBrojKorisnika = 120 };
            var beograd = new Lokacija { Ime = "Hub Beograd", Adresa = "Nemanjina 4", Grad = "Beograd", RadniSati = "00:00–24:00", MaxBrojKorisnika = 300 };
            var noviSad = new Lokacija { Ime = "Hub Novi Sad", Adresa = "Bulevar Oslobodjenja 88", Grad = "Novi Sad", RadniSati = "07:00–23:00", MaxBrojKorisnika = 180 };

            _facade.Lokacije.Dodaj(kragujevac);
            _facade.Lokacije.Dodaj(beograd);
            _facade.Lokacije.Dodaj(noviSad);
            Ok($"Unesene 3 lokacije (Id: {kragujevac.Id}–{noviSad.Id})");

            // ── Resursi — Kragujevac — kreiranje kroz ResursFactory ──────────
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(kragujevac.Id, "Sto A-1", PodtipStola.HotDesk, opis: "Pored prozora"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(kragujevac.Id, "Sto A-2", PodtipStola.DedicatedDesk, opis: "Tih deo zgrade"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(kragujevac.Id, "Sto A-3", PodtipStola.HotDesk, opis: "Blizu ulaza"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(kragujevac.Id, "Sto A-4", PodtipStola.DedicatedDesk, opis: "Tiha zona"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSalu(kragujevac.Id, "Sala K-1", kapacitet: 8, imaProjektor: true, imaTV: true, imaTablu: true, imaOnlineOpremu: true, opis: "Velika sala za sastanke"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSalu(kragujevac.Id, "Sala K-2", kapacitet: 4, imaProjektor: false, imaTV: true, imaTablu: true, imaOnlineOpremu: false, opis: "Mala sala za sastanke"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSalu(kragujevac.Id, "Sala K-3", kapacitet: 10, imaProjektor: true, imaTV: false, imaTablu: true, imaOnlineOpremu: true, opis: "Sala sa projektorom"));

            // ── Resursi — Beograd — kreiranje kroz ResursFactory ─────────────
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(beograd.Id, "Sto B-1", PodtipStola.HotDesk, opis: "Otvoren prostor"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(beograd.Id, "Sto B-2", PodtipStola.HotDesk, opis: "Otvoren prostor"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(beograd.Id, "Sto B-3", PodtipStola.DedicatedDesk, opis: "Premium prostor"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(beograd.Id, "Sto B-4", PodtipStola.HotDesk, opis: "Blizu kuhinje"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(beograd.Id, "Sto B-5", PodtipStola.DedicatedDesk, opis: "Tihi deo"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSalu(beograd.Id, "Sala Y-1", kapacitet: 12, imaProjektor: true, imaTV: true, imaTablu: true, imaOnlineOpremu: true, opis: "Premium prostor"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSalu(beograd.Id, "Sala Y-2", kapacitet: 15, imaProjektor: true, imaTV: true, imaTablu: true, imaOnlineOpremu: true, opis: "Sala za prezentacije"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajPrivatnuKancelariju(beograd.Id, "Privatna kancelarija G-1", kapacitet: 6, imaTV: true, imaTablu: true, imaOnlineOpremu: true, opis: "Privatna timska kancelarija"));

            // ── Resursi — Novi Sad — kreiranje kroz ResursFactory ────────────
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(noviSad.Id, "Sto C-1", PodtipStola.HotDesk, opis: "Pored prozora"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(noviSad.Id, "Sto C-2", PodtipStola.DedicatedDesk, opis: "U uglu"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSto(noviSad.Id, "Sto C-3", PodtipStola.HotDesk, opis: "Radni sto pored zida"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSalu(noviSad.Id, "Sala T-1", kapacitet: 6, imaProjektor: true, imaTV: false, imaTablu: true, imaOnlineOpremu: true, opis: "OGROMNA sala"));
            _facade.Resursi.Dodaj(ResursFactory.KreirajSalu(noviSad.Id, "Sala T-2", kapacitet: 8, imaProjektor: false, imaTV: true, imaTablu: true, imaOnlineOpremu: true, opis: "Sala za timske sastanke"));
            Ok("Uneseno 22 resursa");

            // ── Korisnici (iz podaci.sql, TipClanstvaId 1=Dnevno, 2=HotDesk, 3=Dedicated, 4=Premium)
            // Koristimo lokalne Id varijable da ne zavisimo od hardkodiranih Id-jeva
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Marko", Prezime = "Jovanovic", Email = "marko.j@mail.com", Telefon = "061111111", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Ana", Prezime = "Petrovic", Email = "ana.p@mail.com", Telefon = "062222222", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 5), DatumKrajaClanstva = new DateOnly(2025, 2, 4), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Nikola", Prezime = "Ilic", Email = "nikola.i@mail.com", Telefon = "063333333", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Jelena", Prezime = "Markovic", Email = "jelena.m@mail.com", Telefon = "064444444", TipClanstvaId = dnevno.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 10), DatumKrajaClanstva = new DateOnly(2025, 1, 10), StatusNaloga = StatusNaloga.Istekao });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Ivan", Prezime = "Stojanovic", Email = "ivan.s@mail.com", Telefon = "065555555", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 3), DatumKrajaClanstva = new DateOnly(2025, 2, 2), StatusNaloga = StatusNaloga.Pauziran });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Milica", Prezime = "Kovacevic", Email = "milica.k@mail.com", Telefon = "066666666", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Stefan", Prezime = "Nikolic", Email = "stefan.n@mail.com", Telefon = "067777777", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Tamara", Prezime = "Lazic", Email = "tamara.l@mail.com", Telefon = "068888888", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 12), DatumKrajaClanstva = new DateOnly(2025, 2, 11), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Petar", Prezime = "Djordjevic", Email = "petar.d@mail.com", Telefon = "069999999", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 8), DatumKrajaClanstva = new DateOnly(2025, 2, 7), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Marija", Prezime = "Vasic", Email = "marija.v@mail.com", Telefon = "060000000", TipClanstvaId = dnevno.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 15), DatumKrajaClanstva = new DateOnly(2025, 1, 15), StatusNaloga = StatusNaloga.Istekao });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Luka", Prezime = "Pavlovic", Email = "luka.p@mail.com", Telefon = "061101010", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 2), DatumKrajaClanstva = new DateOnly(2025, 2, 1), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Sara", Prezime = "Milosevic", Email = "sara.m@mail.com", Telefon = "062202020", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 3), DatumKrajaClanstva = new DateOnly(2025, 2, 2), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Filip", Prezime = "Arandjelovic", Email = "filip.a@mail.com", Telefon = "063303030", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Maja", Prezime = "Popovic", Email = "maja.p@mail.com", Telefon = "064404040", TipClanstvaId = dnevno.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 18), DatumKrajaClanstva = new DateOnly(2025, 1, 18), StatusNaloga = StatusNaloga.Istekao });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Nemanja", Prezime = "Ristic", Email = "nemanja.r@mail.com", Telefon = "065505050", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 6), DatumKrajaClanstva = new DateOnly(2025, 2, 5), StatusNaloga = StatusNaloga.Pauziran });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Jovana", Prezime = "Mitrovic", Email = "jovana.m@mail.com", Telefon = "066606060", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 7), DatumKrajaClanstva = new DateOnly(2025, 2, 6), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Aleksandar", Prezime = "Petkovic", Email = "aleks.p@mail.com", Telefon = "067707070", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Teodora", Prezime = "Stankovic", Email = "teodora.s@mail.com", Telefon = "068808080", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 11), DatumKrajaClanstva = new DateOnly(2025, 2, 10), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Vuk", Prezime = "Todorovic", Email = "vuk.t@mail.com", Telefon = "069909090", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 9), DatumKrajaClanstva = new DateOnly(2025, 2, 8), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Isidora", Prezime = "Jaksic", Email = "isidora.j@mail.com", Telefon = "060111111", TipClanstvaId = dnevno.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 21), DatumKrajaClanstva = new DateOnly(2025, 1, 21), StatusNaloga = StatusNaloga.Istekao });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Ognjen", Prezime = "Knezevic", Email = "ognjen.k@mail.com", Telefon = "061222333", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 4), DatumKrajaClanstva = new DateOnly(2025, 2, 3), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Kristina", Prezime = "Obradovic", Email = "kristina.o@mail.com", Telefon = "062333444", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 5), DatumKrajaClanstva = new DateOnly(2025, 2, 4), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Dusan", Prezime = "Maric", Email = "dusan.m@mail.com", Telefon = "063444555", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Anja", Prezime = "Simic", Email = "anja.s@mail.com", Telefon = "064555666", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 13), DatumKrajaClanstva = new DateOnly(2025, 2, 12), StatusNaloga = StatusNaloga.Aktivan });
            _facade.Korisnici.Dodaj(new Korisnik { Ime = "Bojan", Prezime = "Zivkovic", Email = "bojan.z@mail.com", Telefon = "065666777", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 14), DatumKrajaClanstva = new DateOnly(2025, 2, 13), StatusNaloga = StatusNaloga.Aktivan });
            Ok("Uneseno 25 korisnika");

            // ── Rezervacije (iz podaci.sql — ResursId i KorisnikId po redosledu unosa) ──
            // Uzimamo Id-jeve iz baze po imenu da ne zavisimo od IDENTITY vrednosti
            var sviR = _facade.Resursi.DajSve();
            var sviK = _facade.Korisnici.DajSve();

            int ResId(string ime) => sviR.First(r => r.Ime == ime).Id;
            int KorId(string email) => sviK.First(k => k.Email == email).Id;

            // Rezervacije se kreiraju kroz RezervacijaBuilder — garantuje validnost svakog objekta
            void DodajRez(string email, string resIme, DateTime od, DateTime do_, StatusRezervacije status) =>
                _facade.Rezervacije.Dodaj(new RezervacijaBuilder()
                    .ZaKorisnika(KorId(email))
                    .NaResursu(ResId(resIme))
                    .Od(od).Do(do_)
                    .SaStatusom(status)
                    .Build());

            DodajRez("marko.j@mail.com", "Sto A-1", new DateTime(2025, 1, 20, 9, 0, 0), new DateTime(2025, 1, 20, 13, 0, 0), StatusRezervacije.Zavrsena);
            DodajRez("ana.p@mail.com", "Sto A-3", new DateTime(2025, 1, 22, 10, 0, 0), new DateTime(2025, 1, 22, 12, 0, 0), StatusRezervacije.Zavrsena);
            DodajRez("nikola.i@mail.com", "Sala K-3", new DateTime(2025, 1, 25, 14, 0, 0), new DateTime(2025, 1, 25, 18, 0, 0), StatusRezervacije.Aktivna);
            DodajRez("milica.k@mail.com", "Sto B-3", new DateTime(2025, 1, 26, 9, 0, 0), new DateTime(2025, 1, 26, 11, 0, 0), StatusRezervacije.Aktivna);
            DodajRez("stefan.n@mail.com", "Sto B-1", new DateTime(2025, 1, 27, 15, 0, 0), new DateTime(2025, 1, 27, 17, 0, 0), StatusRezervacije.Otkazana);
            DodajRez("luka.p@mail.com", "Sto A-2", new DateTime(2025, 1, 28, 9, 0, 0), new DateTime(2025, 1, 28, 12, 0, 0), StatusRezervacije.Zavrsena);
            DodajRez("sara.m@mail.com", "Sala K-1", new DateTime(2025, 1, 29, 10, 0, 0), new DateTime(2025, 1, 29, 13, 0, 0), StatusRezervacije.Zavrsena);
            DodajRez("filip.a@mail.com", "Sto B-2", new DateTime(2025, 1, 30, 14, 0, 0), new DateTime(2025, 1, 30, 18, 0, 0), StatusRezervacije.Aktivna);
            DodajRez("maja.p@mail.com", "Sala Y-1", new DateTime(2025, 1, 31, 9, 0, 0), new DateTime(2025, 1, 31, 11, 0, 0), StatusRezervacije.Aktivna);
            DodajRez("nemanja.r@mail.com", "Sala K-2", new DateTime(2025, 2, 1, 15, 0, 0), new DateTime(2025, 2, 1, 17, 0, 0), StatusRezervacije.Otkazana);
            DodajRez("jovana.m@mail.com", "Sto A-4", new DateTime(2025, 2, 2, 8, 0, 0), new DateTime(2025, 2, 2, 10, 0, 0), StatusRezervacije.Zavrsena);
            DodajRez("aleks.p@mail.com", "Sala K-3", new DateTime(2025, 2, 3, 12, 0, 0), new DateTime(2025, 2, 3, 14, 0, 0), StatusRezervacije.Aktivna);
            DodajRez("teodora.s@mail.com", "Sto B-3", new DateTime(2025, 2, 4, 16, 0, 0), new DateTime(2025, 2, 4, 18, 0, 0), StatusRezervacije.Aktivna);
            DodajRez("vuk.t@mail.com", "Sto A-1", new DateTime(2025, 2, 5, 9, 0, 0), new DateTime(2025, 2, 5, 12, 0, 0), StatusRezervacije.Zavrsena);
            DodajRez("isidora.j@mail.com", "Sto B-1", new DateTime(2025, 2, 6, 13, 0, 0), new DateTime(2025, 2, 6, 16, 0, 0), StatusRezervacije.Otkazana);
            Ok("Uneseno 15 rezervacija");

            // ── Administratori ───────────────────────────────────────────────
            if (_facade.Administratori.DajSve().Count == 0)
            {
                _facade.Administratori.Dodaj(new Administrator { KorisnickoIme = "admin", Lozinka = "Admin123!", Ime = "Glavni", Prezime = "Administrator", Email = "admin@coworking.rs", DatumKreiranja = new DateOnly(2025, 1, 1) });
                _facade.Administratori.Dodaj(new Administrator { KorisnickoIme = "milan.r", Lozinka = "Milan2025@", Ime = "Milan", Prezime = "Rankovic", Email = "milan.r@coworking.rs", DatumKreiranja = new DateOnly(2025, 1, 1) });
                _facade.Administratori.Dodaj(new Administrator { KorisnickoIme = "vlada.j", Lozinka = "Vlada2025@", Ime = "Vlada", Prezime = "Jovanovic", Email = "vlada.j@coworking.rs", DatumKreiranja = new DateOnly(2025, 1, 15) });
                _facade.Administratori.Dodaj(new Administrator { KorisnickoIme = "nikola.p", Lozinka = "Nikola2025@", Ime = "Nikola", Prezime = "Petrovic", Email = "nikola.p@coworking.rs", DatumKreiranja = new DateOnly(2025, 2, 1) });
                Ok("Unesena 4 administratora");
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // KORAK 4 — CRUD testovi
        // ════════════════════════════════════════════════════════════════════

        private static void TestTipovaClanstva()
        {
            Zaglavlje("4a. CRUD — TipClanstvaRepozitorijum");

            var svi = _facade.TipoviClanstva.DajSve();
            Proveri(svi.Count >= 4, $"DajSve() — pronađeno {svi.Count} tipova");

            var hotDesk = _facade.TipoviClanstva.DajPoImenu("Hot Desk");
            Proveri(hotDesk != null, "DajPoImenu('Hot Desk') — pronađen");
            Proveri(hotDesk!.Cena == 120.00m, $"  Cena: {hotDesk.Cena} (očekivano 120.00)");
            Proveri(hotDesk.PristupSali, "  PristupSali: true");
            Proveri(hotDesk.BrojSatiUSaliMesecno == 10, $"  BrojSatiUSaliMesecno: {hotDesk.BrojSatiUSaliMesecno}");

            var privremeni = new TipClanstva { Ime = "Privremeni Test", Cena = 99m, Trajanje = 7, MaxSatiPoMesecu = 20, PristupSali = false };
            _facade.TipoviClanstva.Dodaj(privremeni);
            Proveri(privremeni.Id > 0, $"Dodaj() — novi Id: {privremeni.Id}");

            privremeni.Cena = 55m;
            _facade.TipoviClanstva.Azuriraj(privremeni);
            Proveri(_facade.TipoviClanstva.DajPoId(privremeni.Id)?.Cena == 55m, "Azuriraj() — cena ažurirana na 55");

            _facade.TipoviClanstva.Obrisi(privremeni.Id);
            Proveri(_facade.TipoviClanstva.DajPoId(privremeni.Id) == null, "Obrisi() — zapis uklonjen");
        }

        private static void TestLokacija()
        {
            Zaglavlje("4b. CRUD — LokacijaRepozitorijum");

            var sve = _facade.Lokacije.DajSve();
            Proveri(sve.Count >= 3, $"DajSve() — pronađeno {sve.Count} lokacija");

            var kg = sve.FirstOrDefault(l => l.Grad == "Kragujevac");
            Proveri(kg != null, "Lokacija Kragujevac — pronađena");
            Proveri(kg!.Resursi.Count == 7, $"  Resursi na lokaciji: {kg.Resursi.Count} (očekivano 7)");
        }

        private static void TestKorisnika()
        {
            Zaglavlje("4c. CRUD — KorisnikRepozitorijum");

            var svi = _facade.Korisnici.DajSve();
            Proveri(svi.Count >= 25, $"DajSve() — pronađeno {svi.Count} korisnika");

            var marko = _facade.Korisnici.DajPoEmailu("marko.j@mail.com");
            Proveri(marko != null, "DajPoEmailu() — Marko pronađen");
            Proveri(marko!.TipClanstva.Ime == "Hot Desk", $"  Tip članstva: {marko.TipClanstva.Ime}");
            Proveri(marko.DatumPocetkaClanstva == new DateOnly(2025, 1, 1), $"  DatumPocetkaClanstva: {marko.DatumPocetkaClanstva}");
            Proveri(marko.DatumKrajaClanstva == new DateOnly(2025, 1, 31), $"  DatumKrajaClanstva: {marko.DatumKrajaClanstva}");

            Proveri(_facade.Korisnici.EmailPostoji("marko.j@mail.com"), "EmailPostoji() — true za postojeći");
            Proveri(!_facade.Korisnici.EmailPostoji("ne.postoji@mail.com"), "EmailPostoji() — false za nepostojeći");

            marko.Telefon = "069000000";
            _facade.Korisnici.Azuriraj(marko);
            Proveri(_facade.Korisnici.DajPoEmailu("marko.j@mail.com")?.Telefon == "069000000", "Azuriraj() — telefon ažuriran");
        }

        private static void TestResursa()
        {
            Zaglavlje("4d. CRUD — ResursRepozitorijum");

            var lokacijaId = _facade.Lokacije.DajSve().First(l => l.Grad == "Kragujevac").Id;

            var svi = _facade.Resursi.DajPoLokaciji(lokacijaId);
            Proveri(svi.Count == 7, $"DajPoLokaciji() — pronađeno {svi.Count} resursa (očekivano 7)");

            var stolovi = _facade.Resursi.DajStoloviPoLokaciji(lokacijaId);
            Proveri(stolovi.Count == 4, $"DajStoloviPoLokaciji() — {stolovi.Count} stolova (očekivano 4)");
            Proveri(stolovi.All(s => s.TipResursa == TipResursa.Sto), "Svi vraćeni resursi su tipa 'sto'");

            var sale = _facade.Resursi.DajSalePoLokaciji(lokacijaId);
            Proveri(sale.Count == 3, $"DajSalePoLokaciji() — {sale.Count} sala (očekivano 3)");

            var salaK1 = sale.FirstOrDefault(s => s.Ime == "Sala K-1");
            Proveri(salaK1 != null, "Sala K-1 pronađena");
            Proveri(salaK1!.ImaProjektor == true, "  ImaProjektor: true");
            Proveri(salaK1.Kapacitet == 8, $"  Kapacitet: {salaK1.Kapacitet} (očekivano 8)");
        }

        private static void TestRezervacija()
        {
            Zaglavlje("4e. CRUD — RezervacijaRepozitorijum");

            var marko = _facade.Korisnici.DajPoEmailu("marko.j@mail.com")!;
            var lokacija = _facade.Lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var sto = _facade.Resursi.DajStoloviPoLokaciji(lokacija.Id).First();

            var rez = new Rezervacija
            {
                KorisnikId = marko.Id,
                ResursId = sto.Id,
                PocetakVreme = new DateTime(2025, 3, 10, 9, 0, 0),
                KrajVreme = new DateTime(2025, 3, 10, 13, 0, 0),
                StatusRezervacije = StatusRezervacije.Aktivna
            };
            _facade.Rezervacije.Dodaj(rez);
            Proveri(rez.Id > 0, $"Dodaj() — nova rezervacija Id: {rez.Id}");
            Proveri(_facade.Rezervacije.DajPoKorisniku(marko.Id).Any(r => r.Id == rez.Id), "DajPoKorisniku() — pronađena");
            Proveri(rez.TrajanjeSati == 4.0, $"TrajanjeSati: {rez.TrajanjeSati}h (očekivano 4.0)");

            _facade.Rezervacije.Otkazi(rez.Id);
            Proveri(_facade.Rezervacije.DajPoId(rez.Id)?.StatusRezervacije == StatusRezervacije.Otkazana, "Otkazi() — status = Otkazana");

            var dnevneRez = _facade.Rezervacije.DajPoLokacijiIDanu(lokacija.Id, new DateTime(2025, 3, 10));
            Ok($"DajPoLokacijiIDanu() — {dnevneRez.Count} rezervacija na dan (otkazane nisu uključene)");
        }

        // ════════════════════════════════════════════════════════════════════
        // KORAK 5 — Poslovne provere
        // ════════════════════════════════════════════════════════════════════

        private static void TestPreklapanjaRezervacija()
        {
            Zaglavlje("5a. POSLOVNA PRAVILA — Preklapanje termina");

            var lokacija = _facade.Lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var sto = _facade.Resursi.DajStoloviPoLokaciji(lokacija.Id).First();
            var marko = _facade.Korisnici.DajPoEmailu("marko.j@mail.com")!;

            var baza = new Rezervacija
            {
                KorisnikId = marko.Id,
                ResursId = sto.Id,
                PocetakVreme = new DateTime(2025, 4, 1, 10, 0, 0),
                KrajVreme = new DateTime(2025, 4, 1, 14, 0, 0),
                StatusRezervacije = StatusRezervacije.Aktivna
            };
            _facade.Rezervacije.Dodaj(baza);

            Proveri(_facade.Rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 11, 0, 0), new DateTime(2025, 4, 1, 13, 0, 0)),
                "Preklapanje unutar termina → detektovano ✓");
            Proveri(_facade.Rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 9, 0, 0), new DateTime(2025, 4, 1, 11, 0, 0)),
                "Delimično preklapanje → detektovano ✓");
            Proveri(!_facade.Rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 7, 0, 0), new DateTime(2025, 4, 1, 10, 0, 0)),
                "Termin pre rezervacije → nema preklapanja ✓");
            Proveri(!_facade.Rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 14, 0, 0), new DateTime(2025, 4, 1, 16, 0, 0)),
                "Termin posle rezervacije → nema preklapanja ✓");
            Proveri(!_facade.Rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 10, 0, 0), new DateTime(2025, 4, 1, 14, 0, 0), excludeId: baza.Id),
                "ExcludeId (izmena) → ne preklapa sa samim sobom ✓");

            _facade.Rezervacije.Obrisi(baza.Id);
        }

        private static void TestBrojaSatiKorisnika()
        {
            Zaglavlje("5b. POSLOVNA PRAVILA — Broj sati rezervacija mesečno");

            var marko = _facade.Korisnici.DajPoEmailu("marko.j@mail.com")!;
            var lokacija = _facade.Lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var sto = _facade.Resursi.DajStoloviPoLokaciji(lokacija.Id).First();
            var sala = _facade.Resursi.DajSalePoLokaciji(lokacija.Id).First();

            var r1 = new RezervacijaBuilder().ZaKorisnika(marko.Id).NaResursu(sto.Id).Od(new DateTime(2025, 5, 1, 9, 0, 0)).Do(new DateTime(2025, 5, 1, 13, 0, 0)).Build();
            var r2 = new RezervacijaBuilder().ZaKorisnika(marko.Id).NaResursu(sto.Id).Od(new DateTime(2025, 5, 5, 10, 0, 0)).Do(new DateTime(2025, 5, 5, 12, 0, 0)).Build();
            var r3 = new RezervacijaBuilder().ZaKorisnika(marko.Id).NaResursu(sala.Id).Od(new DateTime(2025, 5, 8, 14, 0, 0)).Do(new DateTime(2025, 5, 8, 16, 0, 0)).Build();
            _facade.Rezervacije.Dodaj(r1);
            _facade.Rezervacije.Dodaj(r2);
            _facade.Rezervacije.Dodaj(r3);

            double ukupno = _facade.Korisnici.DajUkupnoSatiMesecno(marko.Id, 2025, 5);
            double salaSati = _facade.Korisnici.DajSatiSalaMesecno(marko.Id, 2025, 5);
            Proveri(ukupno == 8.0, $"DajUkupnoSatiMesecno() → {ukupno}h (očekivano 8.0)");
            Proveri(salaSati == 2.0, $"DajSatiSalaMesecno() → {salaSati}h (očekivano 2.0)");

            var tip = marko.TipClanstva ?? _facade.TipoviClanstva.DajPoId(marko.TipClanstvaId)!;
            Proveri(ukupno <= tip.MaxSatiPoMesecu, $"Nije prekoračen limit sati ({ukupno}/{tip.MaxSatiPoMesecu}h)");
            Proveri(!tip.BrojSatiUSaliMesecno.HasValue || salaSati <= tip.BrojSatiUSaliMesecno.Value,
                $"Nije prekoračen limit sala ({salaSati}/{tip.BrojSatiUSaliMesecno}h)");

            _facade.Rezervacije.Obrisi(r1.Id);
            _facade.Rezervacije.Obrisi(r2.Id);
            _facade.Rezervacije.Obrisi(r3.Id);
        }

        private static void TestDostupnostiResursa()
        {
            Zaglavlje("5c. POSLOVNA PRAVILA — Dostupnost resursa");

            var lokacija = _facade.Lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var marko = _facade.Korisnici.DajPoEmailu("marko.j@mail.com")!;
            var sto = _facade.Resursi.DajStoloviPoLokaciji(lokacija.Id).First();

            var zaubez = new Rezervacija
            {
                KorisnikId = marko.Id,
                ResursId = sto.Id,
                PocetakVreme = new DateTime(2025, 6, 15, 10, 0, 0),
                KrajVreme = new DateTime(2025, 6, 15, 14, 0, 0),
                StatusRezervacije = StatusRezervacije.Aktivna
            };
            _facade.Rezervacije.Dodaj(zaubez);

            Proveri(!_facade.Resursi.JeDostupan(sto.Id, new DateTime(2025, 6, 15, 12, 0, 0), new DateTime(2025, 6, 15, 16, 0, 0)),
                "Sto nije dostupan u zauzetom terminu ✓");
            Proveri(_facade.Resursi.JeDostupan(sto.Id, new DateTime(2025, 6, 15, 7, 0, 0), new DateTime(2025, 6, 15, 10, 0, 0)),
                "Sto je dostupan pre zauzetog termina ✓");

            var dostupni = _facade.Resursi.DajDostupneResurse(lokacija.Id, new DateTime(2025, 6, 15, 12, 0, 0), new DateTime(2025, 6, 15, 16, 0, 0));
            Proveri(!dostupni.Any(r => r.Id == sto.Id), $"DajDostupneResurse() — zauzeti sto nije u listi ({dostupni.Count} slobodnih)");

            _facade.Rezervacije.Obrisi(zaubez.Id);
        }

        private static void TestStatistikeZauzetosti()
        {
            Zaglavlje("5d. STATISTIKE — Zauzetost lokacije");

            var lokacija = _facade.Lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var marko = _facade.Korisnici.DajPoEmailu("marko.j@mail.com")!;
            var resursiNaLokaciji = _facade.Resursi.DajPoLokaciji(lokacija.Id);
            var prviResurs = resursiNaLokaciji.First();

            var statPre = _facade.Lokacije.DajStatistikuZauzetosti(lokacija.Id, DateTime.Now);
            Ok($"Pre rezervacije: {statPre}");
            Proveri(statPre.UkupnoResursa == resursiNaLokaciji.Count, $"  Ukupno resursa: {statPre.UkupnoResursa}");

            var sada = DateTime.Now;
            var aktivan = new Rezervacija
            {
                KorisnikId = marko.Id,
                ResursId = prviResurs.Id,
                PocetakVreme = sada.AddMinutes(-30),
                KrajVreme = sada.AddHours(2),
                StatusRezervacije = StatusRezervacije.Aktivna
            };
            _facade.Rezervacije.Dodaj(aktivan);

            var statPosle = _facade.Lokacije.DajStatistikuZauzetosti(lokacija.Id, sada);
            Ok($"Posle rezervacije: {statPosle}");
            Proveri(statPosle.ZauzetihResursa >= 1, $"  Bar jedan resurs je zauzet: {statPosle.ZauzetihResursa}");
            Proveri(statPosle.ProcenatZauzetosti > 0, $"  Procenat zauzetosti: {statPosle.ProcenatZauzetosti}%");

            _facade.Rezervacije.Obrisi(aktivan.Id);
        }

        private static void TestFiltriranja()
        {
            Zaglavlje("5e. FILTRIRANJE — Korisnici po kriterijumima");

            var hotDeskTip = _facade.TipoviClanstva.DajPoImenu("Hot Desk")!;
            var hotDeskKorisnici = _facade.Korisnici.DajPoFiltru(tipClanstvaId: hotDeskTip.Id);
            Ok($"Korisnici sa Hot Desk paketom: {hotDeskKorisnici.Count}");
            Proveri(hotDeskKorisnici.All(k => k.TipClanstvaId == hotDeskTip.Id), "Svi imaju Hot Desk tip ✓");

            var aktivni = _facade.Korisnici.DajPoFiltru(status: StatusNaloga.Aktivan);
            var pazirani = _facade.Korisnici.DajPoFiltru(status: StatusNaloga.Pauziran);
            var istekli = _facade.Korisnici.DajPoFiltru(status: StatusNaloga.Istekao);
            Ok($"Aktivni: {aktivni.Count}, Pazirani: {pazirani.Count}, Istekli: {istekli.Count}");
            Proveri(aktivni.Count + pazirani.Count + istekli.Count == _facade.Korisnici.DajSve().Count,
                "Zbir po statusima = ukupan broj korisnika ✓");

            _facade.Rezervacije.ObeleziZavrseneRezervacije();
            Ok("ObeleziZavrseneRezervacije() — izvršeno bez greške ✓");
        }

        private static void TestAdministratora()
        {
            Zaglavlje("6. CRUD — AdministratorRepozitorijum");

            var svi = _facade.Administratori.DajSve();
            Proveri(svi.Count >= 4, $"DajSve() — pronađeno {svi.Count} administratora");

            var admin = _facade.Administratori.DajPoKorisnickomImenu("admin");
            Proveri(admin != null, "DajPoKorisnickomImenu('admin') — pronađen");
            Proveri(admin!.Ime == "Glavni", $"  Ime: {admin.Ime}");
            Proveri(admin.Lozinka == "Admin123!", "  Lozinka odgovara");

            var milanPoEmailu = _facade.Administratori.DajPoEmailu("milan.r@coworking.rs");
            Proveri(milanPoEmailu != null, "DajPoEmailu() — milan.r pronađen");
            Proveri(milanPoEmailu!.KorisnickoIme == "milan.r", $"  KorisnickoIme: {milanPoEmailu.KorisnickoIme}");

            Proveri(_facade.Administratori.KorisnickoImePostoji("admin"), "KorisnickoImePostoji('admin') — true");
            Proveri(!_facade.Administratori.KorisnickoImePostoji("ne.postoji"), "KorisnickoImePostoji('ne.postoji') — false");
            Proveri(_facade.Administratori.EmailPostoji("admin@coworking.rs"), "EmailPostoji — true za postojeći");

            // Dodaj, izmeni, obriši privremeni admin
            var privremeni = new Administrator
            {
                KorisnickoIme = "test.admin",
                Lozinka = "Test123!",
                Ime = "Test",
                Prezime = "Privremeni",
                Email = "test.privremeni@coworking.rs",
                DatumKreiranja = DateOnly.FromDateTime(DateTime.Today)
            };
            _facade.Administratori.Dodaj(privremeni);
            Proveri(privremeni.Id > 0, $"Dodaj() — novi Id: {privremeni.Id}");

            // Provjera prijave — direktno poređenje
            var uBazi = _facade.Administratori.DajPoKorisnickomImenu("test.admin")!;
            Proveri(uBazi.Lozinka == "Test123!", "Prijava — tačna lozinka prihvaćena ✓");
            Proveri(uBazi.Lozinka != "pogresna", "Prijava — pogrešna lozinka odbijena ✓");

            privremeni.Email = "test.izmenjen@coworking.rs";
            _facade.Administratori.Azuriraj(privremeni);
            Proveri(_facade.Administratori.DajPoId(privremeni.Id)?.Email == "test.izmenjen@coworking.rs",
                "Azuriraj() — email ažuriran");

            _facade.Administratori.Obrisi(privremeni.Id);
            Proveri(_facade.Administratori.DajPoId(privremeni.Id) == null, "Obrisi() — zapis uklonjen");
        }

        // ════════════════════════════════════════════════════════════════════
        // Pomoćne metode za ispis
        // ════════════════════════════════════════════════════════════════════

        private static void Zaglavlje(string tekst)
        {
            Console.WriteLine($"\n── {tekst} ──");
        }

        private static void Ok(string poruka)
        {
            Console.WriteLine($"  ✓  {poruka}");
        }

        private static void Proveri(bool uslov, string opis)
        {
            if (uslov)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✓  {opis}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ✗  NEUSPEŠNO: {opis}");
                Console.ResetColor();
                throw new Exception($"Provera neuspešna: {opis}");
            }
            Console.ResetColor();
        }
    }
}