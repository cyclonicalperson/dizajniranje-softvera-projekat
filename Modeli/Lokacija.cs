namespace CoWorkingManager.Modeli
{
    // Predstavlja jednu co-working lokaciju (npr. Kragujevac, Beograd, Novi Sad)
    public class Lokacija
    {
        public int Id { get; set; }

        // Naziv lokacije
        public string Naziv { get; set; } = string.Empty;

        // Adresa (ulica i broj)
        public string? Adresa { get; set; }

        // Grad u kom se lokacija nalazi
        public string? Grad { get; set; }

        // Radno vreme lokacije kao tekst, npr. "08:00 - 22:00"
        public string? RadnoVreme { get; set; }

        // Maksimalan broj korisnika koji mogu biti prisutni u isto vreme
        public int MaksimalniKapacitet { get; set; }

        // Opis lokacije (opciono)
        public string? Opis { get; set; }

        // Resursi koji se nalaze na ovoj lokaciji
        public ICollection<Resurs> Resursi { get; set; } = new List<Resurs>();

        public override string ToString() => $"{Naziv} ({Grad})";
    }
}