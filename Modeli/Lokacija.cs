namespace CoWorkingManager.Modeli
{
    // Predstavlja jednu co-working lokaciju (npr. Kragujevac, Beograd, Novi Sad)
    public class Lokacija
    {
        public int Id { get; set; }

        // Ime lokacije
        public string Ime { get; set; } = string.Empty;

        // Adresa (ulica i broj)
        public string Adresa { get; set; } = string.Empty;

        // Grad u kom se lokacija nalazi
        public string Grad { get; set; } = string.Empty;

        // Radno vreme lokacije kao tekst, npr. "08:00 - 22:00"
        public string RadniSati { get; set; } = string.Empty;

        // Maksimalan broj korisnika koji mogu biti prisutni u isto vreme
        public int MaxBrojKorisnika { get; set; }

        // Resursi koji se nalaze na ovoj lokaciji
        public ICollection<Resurs> Resursi { get; set; } = new List<Resurs>();

        public override string ToString() => $"{Ime} ({Grad})";
    }
}