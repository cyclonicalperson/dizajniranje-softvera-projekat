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

        private static KorisnikRepozitorijum _korisnici = null!;
        private static TipClanstvaRepozitorijum _tipoviClanstva = null!;
        private static LokacijaRepozitorijum _lokacije = null!;
        private static ResursRepozitorijum _resursi = null!;
        private static RezervacijaRepozitorijum _rezervacije = null!;
        private static AdministratorRepozitorijum _administratori = null!;

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
                var kontekst = TestKonekcije();

                _korisnici = new KorisnikRepozitorijum(kontekst);
                _tipoviClanstva = new TipClanstvaRepozitorijum(kontekst);
                _lokacije = new LokacijaRepozitorijum(kontekst);
                _resursi = new ResursRepozitorijum(kontekst);
                _rezervacije = new RezervacijaRepozitorijum(kontekst);
                _administratori = new AdministratorRepozitorijum(kontekst);

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

        private static KontekstBaze TestKonekcije()
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

            return kontekst;
        }

        // ════════════════════════════════════════════════════════════════════
        // KORAK 3 — Seed podataka (1:1 sa podaci.sql)
        // ════════════════════════════════════════════════════════════════════

        private static void TestSeedPodataka()
        {
            Zaglavlje("3. UNOS POČETNIH PODATAKA");

            if (_tipoviClanstva.DajSve().Count > 0)
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

            _tipoviClanstva.Dodaj(dnevno);
            _tipoviClanstva.Dodaj(hotDesk);
            _tipoviClanstva.Dodaj(dedicatedDesk);
            _tipoviClanstva.Dodaj(premium);
            Ok($"Unesena 4 tipa članstva (Id: {dnevno.Id}–{premium.Id})");

            // ── Lokacije (iz podaci.sql — bez Opis kolone) ───────────────────
            // Id 1: Kragujevac, Id 2: Beograd, Id 3: Novi Sad
            var kragujevac = new Lokacija { Ime = "Hub Kragujevac Center", Adresa = "Kralja Aleksandra 12", Grad = "Kragujevac", RadniSati = "08:00–22:00", MaxBrojKorisnika = 120 };
            var beograd = new Lokacija { Ime = "Hub Beograd", Adresa = "Nemanjina 4", Grad = "Beograd", RadniSati = "00:00–24:00", MaxBrojKorisnika = 300 };
            var noviSad = new Lokacija { Ime = "Hub Novi Sad", Adresa = "Bulevar Oslobodjenja 88", Grad = "Novi Sad", RadniSati = "07:00–23:00", MaxBrojKorisnika = 180 };

            _lokacije.Dodaj(kragujevac);
            _lokacije.Dodaj(beograd);
            _lokacije.Dodaj(noviSad);
            Ok($"Unesene 3 lokacije (Id: {kragujevac.Id}–{noviSad.Id})");

            // ── Resursi — Kragujevac (Id 1–7 u podaci.sql) ──────────────────
            _resursi.Dodaj(new Resurs { LokacijaId = kragujevac.Id, Ime = "Sto A-1", TipResursa = TipResursa.Sto, Opis = "Pored prozora", PodtipStola = PodtipStola.HotDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = kragujevac.Id, Ime = "Sto A-2", TipResursa = TipResursa.Sto, Opis = "Tih deo zgrade", PodtipStola = PodtipStola.DedicatedDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = kragujevac.Id, Ime = "Sto A-3", TipResursa = TipResursa.Sto, Opis = "Blizu ulaza", PodtipStola = PodtipStola.HotDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = kragujevac.Id, Ime = "Sto A-4", TipResursa = TipResursa.Sto, Opis = "Tiha zona", PodtipStola = PodtipStola.DedicatedDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = kragujevac.Id, Ime = "Sala K-1", TipResursa = TipResursa.Sala, Opis = "Velika sala za sastanke", Kapacitet = 8, ImaProjektor = true, ImaTV = true, ImaTablu = true, ImaOnlineOpremu = true });
            _resursi.Dodaj(new Resurs { LokacijaId = kragujevac.Id, Ime = "Sala K-2", TipResursa = TipResursa.Sala, Opis = "Mala sala za sastanke", Kapacitet = 4, ImaProjektor = false, ImaTV = true, ImaTablu = true, ImaOnlineOpremu = false });
            _resursi.Dodaj(new Resurs { LokacijaId = kragujevac.Id, Ime = "Sala K-3", TipResursa = TipResursa.Sala, Opis = "Sala sa projektorom", Kapacitet = 10, ImaProjektor = true, ImaTV = false, ImaTablu = true, ImaOnlineOpremu = true });

            // ── Resursi — Beograd (Id 8–15 u podaci.sql) ────────────────────
            _resursi.Dodaj(new Resurs { LokacijaId = beograd.Id, Ime = "Sto B-1", TipResursa = TipResursa.Sto, Opis = "Otvoren prostor", PodtipStola = PodtipStola.HotDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = beograd.Id, Ime = "Sto B-2", TipResursa = TipResursa.Sto, Opis = "Otvoren prostor", PodtipStola = PodtipStola.HotDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = beograd.Id, Ime = "Sto B-3", TipResursa = TipResursa.Sto, Opis = "Premium prostor", PodtipStola = PodtipStola.DedicatedDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = beograd.Id, Ime = "Sto B-4", TipResursa = TipResursa.Sto, Opis = "Blizu kuhinje", PodtipStola = PodtipStola.HotDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = beograd.Id, Ime = "Sto B-5", TipResursa = TipResursa.Sto, Opis = "Tihi deo", PodtipStola = PodtipStola.DedicatedDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = beograd.Id, Ime = "Sala Y-1", TipResursa = TipResursa.Sala, Opis = "Premium prostor", Kapacitet = 12, ImaProjektor = true, ImaTV = true, ImaTablu = true, ImaOnlineOpremu = true });
            _resursi.Dodaj(new Resurs { LokacijaId = beograd.Id, Ime = "Sala Y-2", TipResursa = TipResursa.Sala, Opis = "Sala za prezentacije", Kapacitet = 15, ImaProjektor = true, ImaTV = true, ImaTablu = true, ImaOnlineOpremu = true });
            _resursi.Dodaj(new Resurs { LokacijaId = beograd.Id, Ime = "Privatna kancelarija G-1", TipResursa = TipResursa.PrivatnaKancelarija, Opis = "Privatna timska kancelarija", Kapacitet = 6, ImaProjektor = false, ImaTV = true, ImaTablu = true, ImaOnlineOpremu = true });

            // ── Resursi — Novi Sad (Id 16–20 u podaci.sql) ──────────────────
            _resursi.Dodaj(new Resurs { LokacijaId = noviSad.Id, Ime = "Sto C-1", TipResursa = TipResursa.Sto, Opis = "Pored prozora", PodtipStola = PodtipStola.HotDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = noviSad.Id, Ime = "Sto C-2", TipResursa = TipResursa.Sto, Opis = "U uglu", PodtipStola = PodtipStola.DedicatedDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = noviSad.Id, Ime = "Sto C-3", TipResursa = TipResursa.Sto, Opis = "Radni sto pored zida", PodtipStola = PodtipStola.HotDesk });
            _resursi.Dodaj(new Resurs { LokacijaId = noviSad.Id, Ime = "Sala T-1", TipResursa = TipResursa.Sala, Opis = "OGROMNA sala", Kapacitet = 6, ImaProjektor = true, ImaTV = false, ImaTablu = true, ImaOnlineOpremu = true });
            _resursi.Dodaj(new Resurs { LokacijaId = noviSad.Id, Ime = "Sala T-2", TipResursa = TipResursa.Sala, Opis = "Sala za timske sastanke", Kapacitet = 8, ImaProjektor = false, ImaTV = true, ImaTablu = true, ImaOnlineOpremu = true });
            Ok("Uneseno 22 resursa");

            // ── Korisnici (iz podaci.sql, TipClanstvaId 1=Dnevno, 2=HotDesk, 3=Dedicated, 4=Premium)
            // Koristimo lokalne Id varijable da ne zavisimo od hardkodiranih Id-jeva
            _korisnici.Dodaj(new Korisnik { Ime = "Marko", Prezime = "Jovanovic", Email = "marko.j@mail.com", Telefon = "061111111", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Ana", Prezime = "Petrovic", Email = "ana.p@mail.com", Telefon = "062222222", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 5), DatumKrajaClanstva = new DateOnly(2025, 2, 4), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Nikola", Prezime = "Ilic", Email = "nikola.i@mail.com", Telefon = "063333333", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Jelena", Prezime = "Markovic", Email = "jelena.m@mail.com", Telefon = "064444444", TipClanstvaId = dnevno.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 10), DatumKrajaClanstva = new DateOnly(2025, 1, 10), StatusNaloga = StatusNaloga.Istekao });
            _korisnici.Dodaj(new Korisnik { Ime = "Ivan", Prezime = "Stojanovic", Email = "ivan.s@mail.com", Telefon = "065555555", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 3), DatumKrajaClanstva = new DateOnly(2025, 2, 2), StatusNaloga = StatusNaloga.Pauziran });
            _korisnici.Dodaj(new Korisnik { Ime = "Milica", Prezime = "Kovacevic", Email = "milica.k@mail.com", Telefon = "066666666", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Stefan", Prezime = "Nikolic", Email = "stefan.n@mail.com", Telefon = "067777777", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Tamara", Prezime = "Lazic", Email = "tamara.l@mail.com", Telefon = "068888888", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 12), DatumKrajaClanstva = new DateOnly(2025, 2, 11), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Petar", Prezime = "Djordjevic", Email = "petar.d@mail.com", Telefon = "069999999", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 8), DatumKrajaClanstva = new DateOnly(2025, 2, 7), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Marija", Prezime = "Vasic", Email = "marija.v@mail.com", Telefon = "060000000", TipClanstvaId = dnevno.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 15), DatumKrajaClanstva = new DateOnly(2025, 1, 15), StatusNaloga = StatusNaloga.Istekao });
            _korisnici.Dodaj(new Korisnik { Ime = "Luka", Prezime = "Pavlovic", Email = "luka.p@mail.com", Telefon = "061101010", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 2), DatumKrajaClanstva = new DateOnly(2025, 2, 1), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Sara", Prezime = "Milosevic", Email = "sara.m@mail.com", Telefon = "062202020", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 3), DatumKrajaClanstva = new DateOnly(2025, 2, 2), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Filip", Prezime = "Arandjelovic", Email = "filip.a@mail.com", Telefon = "063303030", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Maja", Prezime = "Popovic", Email = "maja.p@mail.com", Telefon = "064404040", TipClanstvaId = dnevno.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 18), DatumKrajaClanstva = new DateOnly(2025, 1, 18), StatusNaloga = StatusNaloga.Istekao });
            _korisnici.Dodaj(new Korisnik { Ime = "Nemanja", Prezime = "Ristic", Email = "nemanja.r@mail.com", Telefon = "065505050", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 6), DatumKrajaClanstva = new DateOnly(2025, 2, 5), StatusNaloga = StatusNaloga.Pauziran });
            _korisnici.Dodaj(new Korisnik { Ime = "Jovana", Prezime = "Mitrovic", Email = "jovana.m@mail.com", Telefon = "066606060", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 7), DatumKrajaClanstva = new DateOnly(2025, 2, 6), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Aleksandar", Prezime = "Petkovic", Email = "aleks.p@mail.com", Telefon = "067707070", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Teodora", Prezime = "Stankovic", Email = "teodora.s@mail.com", Telefon = "068808080", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 11), DatumKrajaClanstva = new DateOnly(2025, 2, 10), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Vuk", Prezime = "Todorovic", Email = "vuk.t@mail.com", Telefon = "069909090", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 9), DatumKrajaClanstva = new DateOnly(2025, 2, 8), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Isidora", Prezime = "Jaksic", Email = "isidora.j@mail.com", Telefon = "060111111", TipClanstvaId = dnevno.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 21), DatumKrajaClanstva = new DateOnly(2025, 1, 21), StatusNaloga = StatusNaloga.Istekao });
            _korisnici.Dodaj(new Korisnik { Ime = "Ognjen", Prezime = "Knezevic", Email = "ognjen.k@mail.com", Telefon = "061222333", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 4), DatumKrajaClanstva = new DateOnly(2025, 2, 3), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Kristina", Prezime = "Obradovic", Email = "kristina.o@mail.com", Telefon = "062333444", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 5), DatumKrajaClanstva = new DateOnly(2025, 2, 4), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Dusan", Prezime = "Maric", Email = "dusan.m@mail.com", Telefon = "063444555", TipClanstvaId = premium.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 1), DatumKrajaClanstva = new DateOnly(2025, 1, 31), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Anja", Prezime = "Simic", Email = "anja.s@mail.com", Telefon = "064555666", TipClanstvaId = hotDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 13), DatumKrajaClanstva = new DateOnly(2025, 2, 12), StatusNaloga = StatusNaloga.Aktivan });
            _korisnici.Dodaj(new Korisnik { Ime = "Bojan", Prezime = "Zivkovic", Email = "bojan.z@mail.com", Telefon = "065666777", TipClanstvaId = dedicatedDesk.Id, DatumPocetkaClanstva = new DateOnly(2025, 1, 14), DatumKrajaClanstva = new DateOnly(2025, 2, 13), StatusNaloga = StatusNaloga.Aktivan });
            Ok("Uneseno 25 korisnika");

            // ── Rezervacije (iz podaci.sql — ResursId i KorisnikId po redosledu unosa) ──
            // Uzimamo Id-jeve iz baze po imenu da ne zavisimo od IDENTITY vrednosti
            var sviR = _resursi.DajSve();
            var sviK = _korisnici.DajSve();

            int ResId(string ime) => sviR.First(r => r.Ime == ime).Id;
            int KorId(string email) => sviK.First(k => k.Email == email).Id;

            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("marko.j@mail.com"), ResursId = ResId("Sto A-1"), PocetakVreme = new DateTime(2025, 1, 20, 9, 0, 0), KrajVreme = new DateTime(2025, 1, 20, 13, 0, 0), StatusRezervacije = StatusRezervacije.Zavrsena });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("ana.p@mail.com"), ResursId = ResId("Sto A-3"), PocetakVreme = new DateTime(2025, 1, 22, 10, 0, 0), KrajVreme = new DateTime(2025, 1, 22, 12, 0, 0), StatusRezervacije = StatusRezervacije.Zavrsena });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("nikola.i@mail.com"), ResursId = ResId("Sala K-3"), PocetakVreme = new DateTime(2025, 1, 25, 14, 0, 0), KrajVreme = new DateTime(2025, 1, 25, 18, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("milica.k@mail.com"), ResursId = ResId("Sto B-3"), PocetakVreme = new DateTime(2025, 1, 26, 9, 0, 0), KrajVreme = new DateTime(2025, 1, 26, 11, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("stefan.n@mail.com"), ResursId = ResId("Sto B-1"), PocetakVreme = new DateTime(2025, 1, 27, 15, 0, 0), KrajVreme = new DateTime(2025, 1, 27, 17, 0, 0), StatusRezervacije = StatusRezervacije.Otkazana });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("luka.p@mail.com"), ResursId = ResId("Sto A-2"), PocetakVreme = new DateTime(2025, 1, 28, 9, 0, 0), KrajVreme = new DateTime(2025, 1, 28, 12, 0, 0), StatusRezervacije = StatusRezervacije.Zavrsena });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("sara.m@mail.com"), ResursId = ResId("Sala K-1"), PocetakVreme = new DateTime(2025, 1, 29, 10, 0, 0), KrajVreme = new DateTime(2025, 1, 29, 13, 0, 0), StatusRezervacije = StatusRezervacije.Zavrsena });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("filip.a@mail.com"), ResursId = ResId("Sto B-2"), PocetakVreme = new DateTime(2025, 1, 30, 14, 0, 0), KrajVreme = new DateTime(2025, 1, 30, 18, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("maja.p@mail.com"), ResursId = ResId("Sala Y-1"), PocetakVreme = new DateTime(2025, 1, 31, 9, 0, 0), KrajVreme = new DateTime(2025, 1, 31, 11, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("nemanja.r@mail.com"), ResursId = ResId("Sala K-2"), PocetakVreme = new DateTime(2025, 2, 1, 15, 0, 0), KrajVreme = new DateTime(2025, 2, 1, 17, 0, 0), StatusRezervacije = StatusRezervacije.Otkazana });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("jovana.m@mail.com"), ResursId = ResId("Sto A-4"), PocetakVreme = new DateTime(2025, 2, 2, 8, 0, 0), KrajVreme = new DateTime(2025, 2, 2, 10, 0, 0), StatusRezervacije = StatusRezervacije.Zavrsena });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("aleks.p@mail.com"), ResursId = ResId("Sala K-3"), PocetakVreme = new DateTime(2025, 2, 3, 12, 0, 0), KrajVreme = new DateTime(2025, 2, 3, 14, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("teodora.s@mail.com"), ResursId = ResId("Sto B-3"), PocetakVreme = new DateTime(2025, 2, 4, 16, 0, 0), KrajVreme = new DateTime(2025, 2, 4, 18, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("vuk.t@mail.com"), ResursId = ResId("Sto A-1"), PocetakVreme = new DateTime(2025, 2, 5, 9, 0, 0), KrajVreme = new DateTime(2025, 2, 5, 12, 0, 0), StatusRezervacije = StatusRezervacije.Zavrsena });
            _rezervacije.Dodaj(new Rezervacija { KorisnikId = KorId("isidora.j@mail.com"), ResursId = ResId("Sto B-1"), PocetakVreme = new DateTime(2025, 2, 6, 13, 0, 0), KrajVreme = new DateTime(2025, 2, 6, 16, 0, 0), StatusRezervacije = StatusRezervacije.Otkazana });
            Ok("Uneseno 15 rezervacija");

            // ── Administratori ───────────────────────────────────────────────
            if (_administratori.DajSve().Count == 0)
            {
                _administratori.Dodaj(new Administrator { KorisnickoIme = "admin", Lozinka = "Admin123!", Ime = "Glavni", Prezime = "Administrator", Email = "admin@coworking.rs", DatumKreiranja = new DateOnly(2025, 1, 1) });
                _administratori.Dodaj(new Administrator { KorisnickoIme = "milan.r", Lozinka = "Milan2025@", Ime = "Milan", Prezime = "Rankovic", Email = "milan.r@coworking.rs", DatumKreiranja = new DateOnly(2025, 1, 1) });
                _administratori.Dodaj(new Administrator { KorisnickoIme = "vlada.j", Lozinka = "Vlada2025@", Ime = "Vlada", Prezime = "Jovanovic", Email = "vlada.j@coworking.rs", DatumKreiranja = new DateOnly(2025, 1, 15) });
                _administratori.Dodaj(new Administrator { KorisnickoIme = "nikola.p", Lozinka = "Nikola2025@", Ime = "Nikola", Prezime = "Petrovic", Email = "nikola.p@coworking.rs", DatumKreiranja = new DateOnly(2025, 2, 1) });
                Ok("Unesena 4 administratora");
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // KORAK 4 — CRUD testovi
        // ════════════════════════════════════════════════════════════════════

        private static void TestTipovaClanstva()
        {
            Zaglavlje("4a. CRUD — TipClanstvaRepozitorijum");

            var svi = _tipoviClanstva.DajSve();
            Proveri(svi.Count >= 4, $"DajSve() — pronađeno {svi.Count} tipova");

            var hotDesk = _tipoviClanstva.DajPoImenu("Hot Desk");
            Proveri(hotDesk != null, "DajPoImenu('Hot Desk') — pronađen");
            Proveri(hotDesk!.Cena == 120.00m, $"  Cena: {hotDesk.Cena} (očekivano 120.00)");
            Proveri(hotDesk.PristupSali, "  PristupSali: true");
            Proveri(hotDesk.BrojSatiUSaliMesecno == 10, $"  BrojSatiUSaliMesecno: {hotDesk.BrojSatiUSaliMesecno}");

            var privremeni = new TipClanstva { Ime = "Privremeni Test", Cena = 99m, Trajanje = 7, MaxSatiPoMesecu = 20, PristupSali = false };
            _tipoviClanstva.Dodaj(privremeni);
            Proveri(privremeni.Id > 0, $"Dodaj() — novi Id: {privremeni.Id}");

            privremeni.Cena = 55m;
            _tipoviClanstva.Azuriraj(privremeni);
            Proveri(_tipoviClanstva.DajPoId(privremeni.Id)?.Cena == 55m, "Azuriraj() — cena ažurirana na 55");

            _tipoviClanstva.Obrisi(privremeni.Id);
            Proveri(_tipoviClanstva.DajPoId(privremeni.Id) == null, "Obrisi() — zapis uklonjen");
        }

        private static void TestLokacija()
        {
            Zaglavlje("4b. CRUD — LokacijaRepozitorijum");

            var sve = _lokacije.DajSve();
            Proveri(sve.Count >= 3, $"DajSve() — pronađeno {sve.Count} lokacija");

            var kg = sve.FirstOrDefault(l => l.Grad == "Kragujevac");
            Proveri(kg != null, "Lokacija Kragujevac — pronađena");
            Proveri(kg!.Resursi.Count == 7, $"  Resursi na lokaciji: {kg.Resursi.Count} (očekivano 7)");
        }

        private static void TestKorisnika()
        {
            Zaglavlje("4c. CRUD — KorisnikRepozitorijum");

            var svi = _korisnici.DajSve();
            Proveri(svi.Count >= 25, $"DajSve() — pronađeno {svi.Count} korisnika");

            var marko = _korisnici.DajPoEmailu("marko.j@mail.com");
            Proveri(marko != null, "DajPoEmailu() — Marko pronađen");
            Proveri(marko!.TipClanstva.Ime == "Hot Desk", $"  Tip članstva: {marko.TipClanstva.Ime}");
            Proveri(marko.DatumPocetkaClanstva == new DateOnly(2025, 1, 1), $"  DatumPocetkaClanstva: {marko.DatumPocetkaClanstva}");
            Proveri(marko.DatumKrajaClanstva == new DateOnly(2025, 1, 31), $"  DatumKrajaClanstva: {marko.DatumKrajaClanstva}");

            Proveri(_korisnici.EmailPostoji("marko.j@mail.com"), "EmailPostoji() — true za postojeći");
            Proveri(!_korisnici.EmailPostoji("ne.postoji@mail.com"), "EmailPostoji() — false za nepostojeći");

            marko.Telefon = "069000000";
            _korisnici.Azuriraj(marko);
            Proveri(_korisnici.DajPoEmailu("marko.j@mail.com")?.Telefon == "069000000", "Azuriraj() — telefon ažuriran");
        }

        private static void TestResursa()
        {
            Zaglavlje("4d. CRUD — ResursRepozitorijum");

            var lokacijaId = _lokacije.DajSve().First(l => l.Grad == "Kragujevac").Id;

            var svi = _resursi.DajPoLokaciji(lokacijaId);
            Proveri(svi.Count == 7, $"DajPoLokaciji() — pronađeno {svi.Count} resursa (očekivano 7)");

            var stolovi = _resursi.DajStoloviPoLokaciji(lokacijaId);
            Proveri(stolovi.Count == 4, $"DajStoloviPoLokaciji() — {stolovi.Count} stolova (očekivano 4)");
            Proveri(stolovi.All(s => s.TipResursa == TipResursa.Sto), "Svi vraćeni resursi su tipa 'sto'");

            var sale = _resursi.DajSalePoLokaciji(lokacijaId);
            Proveri(sale.Count == 3, $"DajSalePoLokaciji() — {sale.Count} sala (očekivano 3)");

            var salaK1 = sale.FirstOrDefault(s => s.Ime == "Sala K-1");
            Proveri(salaK1 != null, "Sala K-1 pronađena");
            Proveri(salaK1!.ImaProjektor == true, "  ImaProjektor: true");
            Proveri(salaK1.Kapacitet == 8, $"  Kapacitet: {salaK1.Kapacitet} (očekivano 8)");
        }

        private static void TestRezervacija()
        {
            Zaglavlje("4e. CRUD — RezervacijaRepozitorijum");

            var marko = _korisnici.DajPoEmailu("marko.j@mail.com")!;
            var lokacija = _lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var sto = _resursi.DajStoloviPoLokaciji(lokacija.Id).First();

            var rez = new Rezervacija
            {
                KorisnikId = marko.Id,
                ResursId = sto.Id,
                PocetakVreme = new DateTime(2025, 3, 10, 9, 0, 0),
                KrajVreme = new DateTime(2025, 3, 10, 13, 0, 0),
                StatusRezervacije = StatusRezervacije.Aktivna
            };
            _rezervacije.Dodaj(rez);
            Proveri(rez.Id > 0, $"Dodaj() — nova rezervacija Id: {rez.Id}");
            Proveri(_rezervacije.DajPoKorisniku(marko.Id).Any(r => r.Id == rez.Id), "DajPoKorisniku() — pronađena");
            Proveri(rez.TrajanjeSati == 4.0, $"TrajanjeSati: {rez.TrajanjeSati}h (očekivano 4.0)");

            _rezervacije.Otkazi(rez.Id);
            Proveri(_rezervacije.DajPoId(rez.Id)?.StatusRezervacije == StatusRezervacije.Otkazana, "Otkazi() — status = Otkazana");

            var dnevneRez = _rezervacije.DajPoLokacijiIDanu(lokacija.Id, new DateTime(2025, 3, 10));
            Ok($"DajPoLokacijiIDanu() — {dnevneRez.Count} rezervacija na dan (otkazane nisu uključene)");
        }

        // ════════════════════════════════════════════════════════════════════
        // KORAK 5 — Poslovne provere
        // ════════════════════════════════════════════════════════════════════

        private static void TestPreklapanjaRezervacija()
        {
            Zaglavlje("5a. POSLOVNA PRAVILA — Preklapanje termina");

            var lokacija = _lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var sto = _resursi.DajStoloviPoLokaciji(lokacija.Id).First();
            var marko = _korisnici.DajPoEmailu("marko.j@mail.com")!;

            var baza = new Rezervacija
            {
                KorisnikId = marko.Id,
                ResursId = sto.Id,
                PocetakVreme = new DateTime(2025, 4, 1, 10, 0, 0),
                KrajVreme = new DateTime(2025, 4, 1, 14, 0, 0),
                StatusRezervacije = StatusRezervacije.Aktivna
            };
            _rezervacije.Dodaj(baza);

            Proveri(_rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 11, 0, 0), new DateTime(2025, 4, 1, 13, 0, 0)),
                "Preklapanje unutar termina → detektovano ✓");
            Proveri(_rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 9, 0, 0), new DateTime(2025, 4, 1, 11, 0, 0)),
                "Delimično preklapanje → detektovano ✓");
            Proveri(!_rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 7, 0, 0), new DateTime(2025, 4, 1, 10, 0, 0)),
                "Termin pre rezervacije → nema preklapanja ✓");
            Proveri(!_rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 14, 0, 0), new DateTime(2025, 4, 1, 16, 0, 0)),
                "Termin posle rezervacije → nema preklapanja ✓");
            Proveri(!_rezervacije.PostojiPreklapanje(sto.Id, new DateTime(2025, 4, 1, 10, 0, 0), new DateTime(2025, 4, 1, 14, 0, 0), excludeId: baza.Id),
                "ExcludeId (izmena) → ne preklapa sa samim sobom ✓");

            _rezervacije.Obrisi(baza.Id);
        }

        private static void TestBrojaSatiKorisnika()
        {
            Zaglavlje("5b. POSLOVNA PRAVILA — Broj sati rezervacija mesečno");

            var marko = _korisnici.DajPoEmailu("marko.j@mail.com")!;
            var lokacija = _lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var sto = _resursi.DajStoloviPoLokaciji(lokacija.Id).First();
            var sala = _resursi.DajSalePoLokaciji(lokacija.Id).First();

            var r1 = new Rezervacija { KorisnikId = marko.Id, ResursId = sto.Id, PocetakVreme = new DateTime(2025, 5, 1, 9, 0, 0), KrajVreme = new DateTime(2025, 5, 1, 13, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna };
            var r2 = new Rezervacija { KorisnikId = marko.Id, ResursId = sto.Id, PocetakVreme = new DateTime(2025, 5, 5, 10, 0, 0), KrajVreme = new DateTime(2025, 5, 5, 12, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna };
            var r3 = new Rezervacija { KorisnikId = marko.Id, ResursId = sala.Id, PocetakVreme = new DateTime(2025, 5, 8, 14, 0, 0), KrajVreme = new DateTime(2025, 5, 8, 16, 0, 0), StatusRezervacije = StatusRezervacije.Aktivna };
            _rezervacije.Dodaj(r1);
            _rezervacije.Dodaj(r2);
            _rezervacije.Dodaj(r3);

            double ukupno = _korisnici.DajUkupnoSatiMesecno(marko.Id, 2025, 5);
            double salaSati = _korisnici.DajSatiSalaMesecno(marko.Id, 2025, 5);
            Proveri(ukupno == 8.0, $"DajUkupnoSatiMesecno() → {ukupno}h (očekivano 8.0)");
            Proveri(salaSati == 2.0, $"DajSatiSalaMesecno() → {salaSati}h (očekivano 2.0)");

            var tip = marko.TipClanstva ?? _tipoviClanstva.DajPoId(marko.TipClanstvaId)!;
            Proveri(ukupno <= tip.MaxSatiPoMesecu, $"Nije prekoračen limit sati ({ukupno}/{tip.MaxSatiPoMesecu}h)");
            Proveri(!tip.BrojSatiUSaliMesecno.HasValue || salaSati <= tip.BrojSatiUSaliMesecno.Value,
                $"Nije prekoračen limit sala ({salaSati}/{tip.BrojSatiUSaliMesecno}h)");

            _rezervacije.Obrisi(r1.Id);
            _rezervacije.Obrisi(r2.Id);
            _rezervacije.Obrisi(r3.Id);
        }

        private static void TestDostupnostiResursa()
        {
            Zaglavlje("5c. POSLOVNA PRAVILA — Dostupnost resursa");

            var lokacija = _lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var marko = _korisnici.DajPoEmailu("marko.j@mail.com")!;
            var sto = _resursi.DajStoloviPoLokaciji(lokacija.Id).First();

            var zaubez = new Rezervacija
            {
                KorisnikId = marko.Id,
                ResursId = sto.Id,
                PocetakVreme = new DateTime(2025, 6, 15, 10, 0, 0),
                KrajVreme = new DateTime(2025, 6, 15, 14, 0, 0),
                StatusRezervacije = StatusRezervacije.Aktivna
            };
            _rezervacije.Dodaj(zaubez);

            Proveri(!_resursi.JeDostupan(sto.Id, new DateTime(2025, 6, 15, 12, 0, 0), new DateTime(2025, 6, 15, 16, 0, 0)),
                "Sto nije dostupan u zauzetom terminu ✓");
            Proveri(_resursi.JeDostupan(sto.Id, new DateTime(2025, 6, 15, 7, 0, 0), new DateTime(2025, 6, 15, 10, 0, 0)),
                "Sto je dostupan pre zauzetog termina ✓");

            var dostupni = _resursi.DajDostupneResurse(lokacija.Id, new DateTime(2025, 6, 15, 12, 0, 0), new DateTime(2025, 6, 15, 16, 0, 0));
            Proveri(!dostupni.Any(r => r.Id == sto.Id), $"DajDostupneResurse() — zauzeti sto nije u listi ({dostupni.Count} slobodnih)");

            _rezervacije.Obrisi(zaubez.Id);
        }

        private static void TestStatistikeZauzetosti()
        {
            Zaglavlje("5d. STATISTIKE — Zauzetost lokacije");

            var lokacija = _lokacije.DajSve().First(l => l.Grad == "Kragujevac");
            var marko = _korisnici.DajPoEmailu("marko.j@mail.com")!;
            var resursiNaLokaciji = _resursi.DajPoLokaciji(lokacija.Id);
            var prviResurs = resursiNaLokaciji.First();

            var statPre = _lokacije.DajStatistikuZauzetosti(lokacija.Id, DateTime.Now);
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
            _rezervacije.Dodaj(aktivan);

            var statPosle = _lokacije.DajStatistikuZauzetosti(lokacija.Id, sada);
            Ok($"Posle rezervacije: {statPosle}");
            Proveri(statPosle.ZauzetihResursa >= 1, $"  Bar jedan resurs je zauzet: {statPosle.ZauzetihResursa}");
            Proveri(statPosle.ProcenatZauzetosti > 0, $"  Procenat zauzetosti: {statPosle.ProcenatZauzetosti}%");

            _rezervacije.Obrisi(aktivan.Id);
        }

        private static void TestFiltriranja()
        {
            Zaglavlje("5e. FILTRIRANJE — Korisnici po kriterijumima");

            var hotDeskTip = _tipoviClanstva.DajPoImenu("Hot Desk")!;
            var hotDeskKorisnici = _korisnici.DajPoFiltru(tipClanstvaId: hotDeskTip.Id);
            Ok($"Korisnici sa Hot Desk paketom: {hotDeskKorisnici.Count}");
            Proveri(hotDeskKorisnici.All(k => k.TipClanstvaId == hotDeskTip.Id), "Svi imaju Hot Desk tip ✓");

            var aktivni = _korisnici.DajPoFiltru(status: StatusNaloga.Aktivan);
            var pazirani = _korisnici.DajPoFiltru(status: StatusNaloga.Pauziran);
            var istekli = _korisnici.DajPoFiltru(status: StatusNaloga.Istekao);
            Ok($"Aktivni: {aktivni.Count}, Pazirani: {pazirani.Count}, Istekli: {istekli.Count}");
            Proveri(aktivni.Count + pazirani.Count + istekli.Count == _korisnici.DajSve().Count,
                "Zbir po statusima = ukupan broj korisnika ✓");

            _rezervacije.ObeleziZavrseneRezervacije();
            Ok("ObeleziZavrseneRezervacije() — izvršeno bez greške ✓");
        }

        private static void TestAdministratora()
        {
            Zaglavlje("6. CRUD — AdministratorRepozitorijum");

            var svi = _administratori.DajSve();
            Proveri(svi.Count >= 4, $"DajSve() — pronađeno {svi.Count} administratora");

            var admin = _administratori.DajPoKorisnickomImenu("admin");
            Proveri(admin != null, "DajPoKorisnickomImenu('admin') — pronađen");
            Proveri(admin!.Ime == "Glavni", $"  Ime: {admin.Ime}");
            Proveri(admin.Lozinka == "Admin123!", "  Lozinka odgovara");

            var milanPoEmailu = _administratori.DajPoEmailu("milan.r@coworking.rs");
            Proveri(milanPoEmailu != null, "DajPoEmailu() — milan.r pronađen");
            Proveri(milanPoEmailu!.KorisnickoIme == "milan.r", $"  KorisnickoIme: {milanPoEmailu.KorisnickoIme}");

            Proveri(_administratori.KorisnickoImePostoji("admin"), "KorisnickoImePostoji('admin') — true");
            Proveri(!_administratori.KorisnickoImePostoji("ne.postoji"), "KorisnickoImePostoji('ne.postoji') — false");
            Proveri(_administratori.EmailPostoji("admin@coworking.rs"), "EmailPostoji — true za postojeći");

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
            _administratori.Dodaj(privremeni);
            Proveri(privremeni.Id > 0, $"Dodaj() — novi Id: {privremeni.Id}");

            // Provjera prijave — direktno poređenje
            var uBazi = _administratori.DajPoKorisnickomImenu("test.admin")!;
            Proveri(uBazi.Lozinka == "Test123!", "Prijava — tačna lozinka prihvaćena ✓");
            Proveri(uBazi.Lozinka != "pogresna", "Prijava — pogrešna lozinka odbijena ✓");

            privremeni.Email = "test.izmenjen@coworking.rs";
            _administratori.Azuriraj(privremeni);
            Proveri(_administratori.DajPoId(privremeni.Id)?.Email == "test.izmenjen@coworking.rs",
                "Azuriraj() — email ažuriran");

            _administratori.Obrisi(privremeni.Id);
            Proveri(_administratori.DajPoId(privremeni.Id) == null, "Obrisi() — zapis uklonjen");
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