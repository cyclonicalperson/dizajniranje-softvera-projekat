namespace CoWorkingManager
{
    // Cita konfiguraciju iz config.txt i pruza naziv lanca i konekcioni string
    // Implementira Singleton obrazac — samo jedna instanca postoji tokom rada aplikacije
    //
    // Format config.txt:
    //   Linija 1: naziv lanca co-working prostora
    //   Linija 2: konekcioni string za bazu podataka
    public class ManagerKonfiguracije
    {
        // ── Singleton ────────────────────────────────────────────────────────

        private static ManagerKonfiguracije? instanca;
        private static readonly object katanac = new object();

        // Vraca jedinu instancu
        public static ManagerKonfiguracije Instanca
        {
            get
            {
                if (instanca == null)
                {
                    lock (katanac)
                    {
                        if (instanca == null)
                            instanca = new ManagerKonfiguracije();
                    }
                }
                return instanca;
            }
        }

        // ── Podaci iz config.txt ─────────────────────────────────────────────

        // Naziv co-working lanca (prva linija config.txt)
        public string NazivLanca { get; private set; } = string.Empty;

        // Konekcioni string za bazu (druga linija config.txt)
        public string KonekcioniString { get; private set; } = string.Empty;

        // ── Privatni konstruktor ─────────────────────────────────────────────

        private ManagerKonfiguracije()
        {
            UcitajKonfiguraciju();
        }

        // Ucitava config.txt iz istog foldera kao i exe fajl
        // Trazi fajl u trenutnom direktorijumu i u AppDomain bazi
        private void UcitajKonfiguraciju()
        {
            // Pokusaj nekoliko lokacija gde config.txt može biti
            string[] moguceLokacije = {
                "config.txt",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"),
                Path.Combine(Directory.GetCurrentDirectory(), "config.txt")
            };

            string? putanja = moguceLokacije.FirstOrDefault(File.Exists);

            if (putanja == null)
                throw new FileNotFoundException(
                    "Fajl config.txt nije pronadjen");

            var linije = File.ReadAllLines(putanja);

            if (linije.Length < 2)
                throw new InvalidDataException(
                    "config.txt mora imati najmanje dve linije: naziv lanca i konekcioni string");

            NazivLanca = linije[0].Trim();
            KonekcioniString = linije[1].Trim();

            if (string.IsNullOrEmpty(NazivLanca))
                throw new InvalidDataException("Naziv lanca u config.txt ne sme biti prazan");

            if (string.IsNullOrEmpty(KonekcioniString))
                throw new InvalidDataException("Konekcioni string u config.txt ne sme biti prazan");
        }
    }
}