using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Logika.Servisi
{
    // Servis za automatski periodicni izvoz mesečnog izvestaja u CSV
    //
    // Upotreba:
    //   // Pokretanje automatskog izvoza svakih sat vremena (ili neku drugi enum):
    //   Izvoznik.Instanca.Pokreni(PeriodIzvoza.SvakiSat);
    //
    //   // Pokretanje sa prilagodjenim intervalom u minutima (npr. svakih 30 minuta):
    //   Izvoznik.Instanca.PokreniSaIntervalomMinuta(30);
    //
    //   // Trenutni rucni izvoz bez cekanja:
    //   string putanja = Izvoznik.Instanca.IzveziSada();
    //
    //   // Zaustavljanje:
    //   Izvoznik.Instanca.Zaustavi();
    //
    // Generisani CSV fajlovi se cuvaju u folderu "Izvestaji" pored exe-a,
    // sa nazivom oblika: izvestaj_2025_01.csv
    public class IzvestajServis
    {
        // Singleton
        private static IzvestajServis? instanca;
        private static readonly object katanac = new();

        public static IzvestajServis Instanca
        {
            get
            {
                if (instanca == null)
                    lock (katanac)
                        instanca ??= new IzvestajServis();
                return instanca;
            }
        }

        private IzvestajServis() { }

        // Periodi izvoza
        public enum PeriodIzvoza
        {
            SvakiSat, // svakih 1h
            Svaki5Sati, // svakih 5h
            SvakiDan, // svakih 24h
            SvakaNedelja, // svakih 7 dana
            SvakiMesec // svakih 30 dana
        }

        private static readonly Dictionary<PeriodIzvoza, TimeSpan> intervali = new()
        {
            { PeriodIzvoza.SvakiSat, TimeSpan.FromHours(1) },
            { PeriodIzvoza.Svaki5Sati, TimeSpan.FromHours(5) },
            { PeriodIzvoza.SvakiDan, TimeSpan.FromDays(1) },
            { PeriodIzvoza.SvakaNedelja, TimeSpan.FromDays(7) },
            { PeriodIzvoza.SvakiMesec, TimeSpan.FromDays(30) },
        };

        // Tajmer svakih 60 sekundi i proverava da li je vreme za izvoz
        // Sledece vreme okidanja se cuva kao DateTime
        private Timer? tajmer;
        private TimeSpan interval;
        private DateTime sledeceOkidanje;
        private bool aktivan;

        private static readonly TimeSpan IntervalProvere = TimeSpan.FromSeconds(60);

        // Pokrece automatski izvoz sa predefinisanim periodom
        // Ako je vec pokrenut, zaustavi prethodni pre pokretanja novog
        public void Pokreni(PeriodIzvoza period)
        {
            PokreniSaIntervalomMinuta((int)intervali[period].TotalMinutes);
        }

        // Pokrece automatski izvoz sa proizvoljnim intervalom u minutima
        // Prosledjivanje -1 pokrece tajmer koji okine jednom odmah i ne ponavlja se
        public void PokreniSaIntervalomMinuta(int intervalMinuta)
        {
            if (intervalMinuta != -1 && intervalMinuta < 1)
                throw new ArgumentException("Interval mora biti najmanje 1 minut (ili -1 za jednokratno)", nameof(intervalMinuta));

            Zaustavi();

            interval = intervalMinuta == -1 ? Timeout.InfiniteTimeSpan : TimeSpan.FromMinutes(intervalMinuta);

            // Izvozi odmah pri pokretanju
            IzveziSada();

            if (intervalMinuta != -1)
            {
                sledeceOkidanje = DateTime.Now + interval;

                // Tajmer tickuje svakih 60s i proverava da li je DateTime.Now >= sledeceOkidanje
                tajmer = new Timer(_ =>
                {
                    if (DateTime.Now >= sledeceOkidanje)
                    {
                        IzveziSada();
                        sledeceOkidanje = DateTime.Now + interval;
                    }
                }, null, IntervalProvere, IntervalProvere);
            }

            aktivan = true;

            var opis = intervalMinuta == -1 ? "jednokratno" : $"interval: {interval}";
            Console.WriteLine($"[Izvoznik] Pokrenut — {opis}");
        }

        // Zaustavlja automatski izvoz
        public void Zaustavi()
        {
            if (aktivan)
            {
                tajmer?.Dispose();
                tajmer = null;
                aktivan = false;
                Console.WriteLine("[Izvoznik] Zaustavljem automatski izvoz.");
            }
        }

        // Izvozi izvestaje za sve mesece koji imaju rezervacije u bazi, ali jos nemaju generisan CSV fajl
        public string IzveziSada()
        {
            var fasada = CoworkingFasada.DajInstancu();

            // Pronadji sve jedinstvene godine/mesece iz baze
            var meseci = fasada.Rezervacije.DajSve()
                .Select(r => new { r.PocetakVreme.Year, r.PocetakVreme.Month })
                .Distinct()
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            string poslednjaPutanja = string.Empty;

            foreach (var mesec in meseci)
            {
                // Preskoci ako fajl vec postoji
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projektRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
                string putanjaFajla = Path.Combine(projektRoot, "Izvestaji", $"izvestaj_{mesec.Year}_{mesec.Month:D2}.csv");

                if (File.Exists(putanjaFajla))
                    continue;

                poslednjaPutanja = IzveziZaMesec(mesec.Year, mesec.Month);
            }

            return poslednjaPutanja;
        }

        // Izvozi izvestaj za proizvoljni mesec
        // Vraca putanju do generisanog CSV fajla
        public string IzveziZaMesec(int godina, int mesec)
        {
            try
            {
                var fasada = CoworkingFasada.DajInstancu();
                var rezervacije = fasada.Rezervacije.DajZaMesecniIzvestaj(godina, mesec);

                var sadrzaj = GenerisiCsv(rezervacije, godina, mesec);
                var putanja = SacuvajFajl(sadrzaj, godina, mesec);

                Console.WriteLine($"[Izvoznik] Izvoz završen: {putanja} ({rezervacije.Count} rezervacija)");
                return putanja;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Izvoznik] Greška pri izvozu: {ex.Message}");
                throw;
            }
        }

        // Generisanje CSV sadrzaja
        private static string GenerisiCsv(List<Rezervacija> rezervacije, int godina, int mesec)
        {
            var sb = new StringBuilder();
            var ic = CultureInfo.InvariantCulture;

            // Zaglavlje izvestaja
            sb.AppendLine("MESECNI IZVESTAJ O KORISCENJU RESURSA I CLANSTAVA");
            sb.AppendLine($"Period:,{mesec:D2}/{godina}");
            sb.AppendLine($"Generisano:,{DateTime.Now.ToString("dd.MM.yyyy HH:mm", ic)}");
            sb.AppendLine($"Ukupno rezervacija:,{rezervacije.Count}");
            sb.AppendLine();

            // Sati koriscenja po korisniku
            sb.AppendLine("SATI KORISCENJA PO KORISNIKU");
            sb.AppendLine("Korisnik,Email,Tip clanstva,Sati ukupno,Sati radna mesta,Sati sale,Sati kancelarije,Broj rezervacija");

            var poKorisniku = rezervacije
                .GroupBy(r => r.Korisnik)
                .OrderByDescending(g => g.Sum(r => r.TrajanjeSati))
                .ToList();

            foreach (var grupa in poKorisniku)
            {
                var k = grupa.Key;
                double ukupnoSati = grupa.Sum(r => r.TrajanjeSati);
                double satiSto = grupa.Where(r => r.Resurs.TipResursa == TipResursa.Sto)
                                          .Sum(r => r.TrajanjeSati);
                double satiSala = grupa.Where(r => r.Resurs.TipResursa == TipResursa.Sala)
                                          .Sum(r => r.TrajanjeSati);
                double satiKanc = grupa.Where(r => r.Resurs.TipResursa == TipResursa.PrivatnaKancelarija)
                                          .Sum(r => r.TrajanjeSati);
                int brojRez = grupa.Count();

                sb.AppendLine(string.Join(",",
                    CsvPolje($"{k.Ime} {k.Prezime}"),
                    CsvPolje(k.Email),
                    CsvPolje(k.TipClanstva?.Ime ?? "—"),
                    ukupnoSati.ToString("F1", ic),
                    satiSto.ToString("F1", ic),
                    satiSala.ToString("F1", ic),
                    satiKanc.ToString("F1", ic),
                    brojRez.ToString(ic)
                ));
            }

            // Suma na dnu
            double sumaUkupno = rezervacije.Sum(r => r.TrajanjeSati);
            sb.AppendLine(string.Join(",",
                "UKUPNO",
                sumaUkupno.ToString("F1", ic),
                rezervacije.Where(r => r.Resurs.TipResursa == TipResursa.Sto).Sum(r => r.TrajanjeSati).ToString("F1", ic),
                rezervacije.Where(r => r.Resurs.TipResursa == TipResursa.Sala).Sum(r => r.TrajanjeSati).ToString("F1", ic),
                rezervacije.Where(r => r.Resurs.TipResursa == TipResursa.PrivatnaKancelarija).Sum(r => r.TrajanjeSati).ToString("F1", ic),
                rezervacije.Count.ToString(ic)
            ));
            sb.AppendLine();

            // Zauzetost resursa
            sb.AppendLine("ZAUZETOST RESURSA");
            sb.AppendLine("Resurs,Lokacija,Tip,Broj rezervacija,Sati ukupno,Prosecno sati po rezervaciji");

            var poResusu = rezervacije
                .GroupBy(r => r.Resurs)
                .OrderBy(g => g.Key.Lokacija?.Ime)
                .ThenBy(g => g.Key.TipResursa)
                .ThenBy(g => g.Key.Ime)
                .ToList();

            foreach (var grupa in poResusu)
            {
                var res = grupa.Key;
                int brojRez = grupa.Count();
                double ukupnoSati = grupa.Sum(r => r.TrajanjeSati);
                double prosecno = brojRez > 0 ? ukupnoSati / brojRez : 0;

                sb.AppendLine(string.Join(",",
                    CsvPolje(res.Ime),
                    CsvPolje(res.Lokacija?.Ime ?? "—"),
                    CsvPolje(res.TipResursa.ToString()),
                    brojRez.ToString(ic),
                    ukupnoSati.ToString("F1", ic),
                    prosecno.ToString("F1", ic)
                ));
            }

            return sb.ToString();
        }

        // Cuvanje fajla
        private static string SacuvajFajl(string sadrzaj, int godina, int mesec)
        {
            // Idemo gore iz bin/Debug/net8.0/ do root-a projekta (gde je .sln) kako se ne bi izgubio folder opet
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projektRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));

            var folder = Path.Combine(projektRoot, "Izvestaji");
            Directory.CreateDirectory(folder);

            // Naziv: izvestaj_2025_01.csv
            // Ako fajl za taj mesec vec postoji, prepisujemo ga (najnoviji podaci)
            var naziv = $"izvestaj_{godina}_{mesec:D2}.csv";
            var putanja = Path.Combine(folder, naziv);

            File.WriteAllText(putanja, sadrzaj, Encoding.UTF8);
            return putanja;
        }

        // Escapuje vrednost za CSV — dodaje navodnike ako vrednost sadrzi zarez ili navodnike
        private static string CsvPolje(string vrednost)
        {
            if (vrednost.Contains(',') || vrednost.Contains('"') || vrednost.Contains('\n'))
                return $"\"{vrednost.Replace("\"", "\"\"")}\"";
            return vrednost;
        }
    }
}