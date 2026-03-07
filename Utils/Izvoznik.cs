using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Utils
{
    // ═══════════════════════════════════════════════════════════════════════
    // Izvoznik — automatski periodični izvoz mesečnog izveštaja u CSV
    // ═══════════════════════════════════════════════════════════════════════
    //
    // Korišćenje:
    //
    //   // Pokretanje automatskog izvoza svakih sat vremena:
    //   Izvoznik.Instanca.Pokreni(PeriodIzvoza.SvatSat);
    //
    //   // Pokretanje sa prilagođenim intervalom (npr. svakih 30 minuta):
    //   Izvoznik.Instanca.PokreniSaIntervalomMinuta(30);
    //
    //   // Trenutni ručni izvoz bez čekanja:
    //   string putanja = Izvoznik.Instanca.IzveziSada();
    //
    //   // Zaustavljanje:
    //   Izvoznik.Instanca.Zaustavi();
    //
    // Generisani CSV fajlovi se čuvaju u folderu "Izvestaji" pored exe-a,
    // sa nazivom oblika: izvestaj_2025_01.csv
    //
    // Svaki fajl sadrži dva odeljka:
    //   1. Sati korišćenja po korisniku (ukupno i po tipu resursa)
    //   2. Zauzetost resursa (broj rezervacija i ukupni sati po resursu)
    public class Izvoznik
    {
        // ── Singleton ────────────────────────────────────────────────────────

        private static Izvoznik? _instanca;
        private static readonly object _katanac = new();

        public static Izvoznik Instanca
        {
            get
            {
                if (_instanca == null)
                    lock (_katanac)
                        _instanca ??= new Izvoznik();
                return _instanca;
            }
        }

        private Izvoznik() { }

        // ── Periodi izvoza ────────────────────────────────────────────────────

        public enum PeriodIzvoza
        {
            SvatSat,        // svakih 1h
            Svaki5Sati,     // svakih 5h
            SvakiDan,       // svakih 24h
            SvakaNedelja,   // svakih 7 dana
            SvakiMesec      // svakih 30 dana
        }

        private static readonly Dictionary<PeriodIzvoza, TimeSpan> _intervali = new()
        {
            { PeriodIzvoza.SvatSat,       TimeSpan.FromHours(1)       },
            { PeriodIzvoza.Svaki5Sati,    TimeSpan.FromHours(5)       },
            { PeriodIzvoza.SvakiDan,      TimeSpan.FromDays(1)        },
            { PeriodIzvoza.SvakaNedelja,  TimeSpan.FromDays(7)        },
            { PeriodIzvoza.SvakiMesec,    TimeSpan.FromDays(30)       },
        };

        // ── Stanje tajmera ────────────────────────────────────────────────────

        private Timer? _tajmer;
        private bool _aktivan;

        // ── Javni API ─────────────────────────────────────────────────────────

        // Pokreće automatski izvoz sa predefinisanim periodom.
        // Ako je već pokrenut, zaustavi prethodni pre pokretanja novog.
        public void Pokreni(PeriodIzvoza period)
        {
            PokreniSaIntervalomSekundi((int)_intervali[period].TotalMinutes);
        }

        // Pokreće automatski izvoz sa proizvoljnim intervalom u sekundama.
        // Prosleđivanje -1 pokreće tajmer koji okine jednom odmah i ne ponavlja se.
        public void PokreniSaIntervalomSekundi(int intervalSekundi)
        {
            if (intervalSekundi != -1 && intervalSekundi < 1)
                throw new ArgumentException("Interval mora biti najmanje 1 sekunda (ili -1 za jednokratno).", nameof(intervalSekundi));

            Zaustavi();

            var interval = intervalSekundi == -1
                ? Timeout.InfiniteTimeSpan   // okine jednom odmah, ne ponavlja
                : TimeSpan.FromSeconds(intervalSekundi);

            // Izvozi odmah pri pokretanju, pa zatim periodično
            _tajmer = new Timer(_ => IzveziSada(), null, TimeSpan.Zero, interval);
            _aktivan = true;

            var opis = intervalSekundi == -1 ? "jednokratno" : $"interval: {interval}";
            Console.WriteLine($"[Izvoznik] Pokrenut — {opis}");
        }

        // Zaustavlja automatski izvoz.
        public void Zaustavi()
        {
            if (_aktivan)
            {
                _tajmer?.Dispose();
                _tajmer = null;
                _aktivan = false;
                Console.WriteLine("[Izvoznik] Zaustavljem automatski izvoz.");
            }
        }

        // Izvozi izveštaj za tekući mesec odmah, bez čekanja na tajmer.
        // Vraća putanju do generisanog CSV fajla.
        public string IzveziSada()
        {
            var sada = DateTime.Now;
            return IzveziZaMesec(sada.Year, sada.Month);
        }

        // Izvozi izveštaj za proizvoljni mesec.
        // Vraća putanju do generisanog CSV fajla.
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

        // ── Generisanje CSV sadržaja ──────────────────────────────────────────

        private static string GenerisiCsv(List<Rezervacija> rezervacije, int godina, int mesec)
        {
            var sb = new StringBuilder();

            // ── Zaglavlje izveštaja ───────────────────────────────────────────
            sb.AppendLine($"MESECNI IZVESTAJ O KORISCENJU RESURSA I CLANSTAVA");
            sb.AppendLine($"Period:,{mesec:D2}/{godina}");
            sb.AppendLine($"Generisano:,{DateTime.Now:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Ukupno rezervacija:,{rezervacije.Count}");
            sb.AppendLine();

            // ── Odeljak 1: Sati korišćenja po korisniku ───────────────────────
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
                    ukupnoSati.ToString("F1"),
                    satiSto.ToString("F1"),
                    satiSala.ToString("F1"),
                    satiKanc.ToString("F1"),
                    brojRez.ToString()
                ));
            }

            // Suma na dnu
            double sumaUkupno = rezervacije.Sum(r => r.TrajanjeSati);
            sb.AppendLine(string.Join(",",
                "UKUPNO", "", "",
                sumaUkupno.ToString("F1"),
                rezervacije.Where(r => r.Resurs.TipResursa == TipResursa.Sto).Sum(r => r.TrajanjeSati).ToString("F1"),
                rezervacije.Where(r => r.Resurs.TipResursa == TipResursa.Sala).Sum(r => r.TrajanjeSati).ToString("F1"),
                rezervacije.Where(r => r.Resurs.TipResursa == TipResursa.PrivatnaKancelarija).Sum(r => r.TrajanjeSati).ToString("F1"),
                rezervacije.Count.ToString()
            ));
            sb.AppendLine();

            // ── Odeljak 2: Zauzetost resursa ──────────────────────────────────
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
                    brojRez.ToString(),
                    ukupnoSati.ToString("F1"),
                    prosecno.ToString("F1")
                ));
            }

            return sb.ToString();
        }

        // ── Čuvanje fajla ─────────────────────────────────────────────────────

        private static string SacuvajFajl(string sadrzaj, int godina, int mesec)
        {
            // Idemo gore iz bin/Debug/net8.0/ do root-a projekta (gde je .sln)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projektRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));

            var folder = Path.Combine(projektRoot, "Izvestaji");
            Directory.CreateDirectory(folder);

            // Naziv: izvestaj_2025_01.csv
            // Ako fajl za taj mesec već postoji, prepisujemo ga (najnoviji podaci)
            var naziv = $"izvestaj_{godina}_{mesec:D2}.csv";
            var putanja = Path.Combine(folder, naziv);

            File.WriteAllText(putanja, sadrzaj, Encoding.UTF8);
            return putanja;
        }

        // Escapuje vrednost za CSV — dodaje navodnike ako vrednost sadrži zarez ili navodnike
        private static string CsvPolje(string vrednost)
        {
            if (vrednost.Contains(',') || vrednost.Contains('"') || vrednost.Contains('\n'))
                return $"\"{vrednost.Replace("\"", "\"\"")}\"";
            return vrednost;
        }
    }
}