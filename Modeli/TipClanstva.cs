namespace CoWorkingManager.Modeli
{
    // Predstavlja paket/tip članstva u co-working prostoru
    // (npr. dnevna karta, fleksibilni sto, fiksni sto, premium...)
    public class TipClanstva
    {
        public int Id { get; set; }

        // Ime paketa clanstva
        public string Ime { get; set; } = string.Empty;

        // Cena paketa u dinarima
        public decimal Cena { get; set; }

        // Trajanje paketa u danima (0 = neograničeno)
        public int Trajanje { get; set; }

        // Maksimalan broj sati rezervacija mesecno (0 = neograničeno)
        public int MaxSatiPoMesecu { get; set; }

        // Da li ovaj paket uključuje pristup salama za sastanke
        public bool PristupSali { get; set; }

        // Maksimalan broj sati korišćenja sala mesecno
        // NULL znaci da sale nisu uključene u paket
        public int BrojSatiUSaliMesecno { get; set; }

        // Korisnici koji imaju ovaj tip članstva
        public ICollection<Korisnik> Korisnici { get; set; } = new List<Korisnik>();

        public override string ToString() => Ime;
    }
}