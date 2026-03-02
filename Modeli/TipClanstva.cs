namespace CoWorkingManager.Modeli
{
    // Predstavlja paket/tip članstva u co-working prostoru
    // (npr. dnevna karta, fleksibilni sto, fiksni sto, premium...)
    public class TipClanstva
    {
        public int Id { get; set; }

        // Naziv paketa članstva
        public string Naziv { get; set; } = string.Empty;

        // Cena paketa u dinarima
        public decimal Cena { get; set; }

        // Trajanje paketa u danima (0 = neograničeno)
        public int TrajanjeDani { get; set; }

        // Maksimalan broj sati rezervacija mesečno
        // 0 znači neograničeno
        public int MaksimalnoSatiMesecno { get; set; }

        // Da li ovaj paket uključuje pristup salama za sastanke
        public bool UkljucujeSale { get; set; }

        // Maksimalan broj sati korišćenja sala mesečno
        // 0 = neograničeno, -1 = sale nisu uključene
        public int MaksimalnoSatiSalaMesecno { get; set; }

        // Korisnici koji imaju ovaj tip članstva
        public ICollection<Korisnik> Korisnici { get; set; } = new List<Korisnik>();

        public override string ToString() => Naziv;
    }
}