namespace CoWorkingManager.Modeli
{
    // Predstavlja paket/tip clanstva (Dnevno, Hot Desk, Dedicated Desk, Premium...)
    // Mapira se na tabelu TipoviClanstava u bazi
    public class TipClanstva
    {
        public int Id { get; set; }

        // Naziv paketa — jedinstven u bazi (UNIQUE constraint)
        public string Ime { get; set; } = string.Empty;

        // Cena paketa
        public decimal Cena { get; set; }

        // Trajanje paketa u danima
        public int Trajanje { get; set; }

        // Maksimalan broj sati rezervacija mesečno (0 = neograničeno)
        public int MaxSatiPoMesecu { get; set; }

        // Da li paket ukljucuje pristup salama za sastanke
        public bool PristupSali { get; set; }

        // Maksimalan broj sati korišćenja sala mesecno
        // NULL znaci da sale nisu uključene u paket
        public int? BrojSatiUSaliMesecno { get; set; }

        // Korisnici koji imaju ovaj tip clanstva
        public ICollection<Korisnik> Korisnici { get; set; } = new List<Korisnik>();

        public override string ToString() => Ime;
    }
}