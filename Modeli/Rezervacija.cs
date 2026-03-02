namespace CoWorkingManager.Modeli
{
    // Predstavlja jednu rezervaciju resursa od strane korisnik
    public class Rezervacija
    {
        public int Id { get; set; }

        // Strani ključ na korisnika koji je napravio rezervaciju
        public int KorisnikId { get; set; }

        // Navigaciono svojstvo ka korisniku
        public Korisnik Korisnik { get; set; } = null!;

        // Strani ključ na rezervisani resurs
        public int ResursId { get; set; }

        // Navigaciono svojstvo ka resursu
        public Resurs Resurs { get; set; } = null!;

        // Datum i vreme početka rezervacije
        public DateTime VremePocetka { get; set; }

        // Datum i vreme završetka rezervacije
        public DateTime VremeZavrsetka { get; set; }

        // Trenutni status rezervacije
        public StatusRezervacije Status { get; set; } = StatusRezervacije.Aktivna;

        // Trajanje rezervacije u satima (izračunato)
        public double TrajanjeSati => (VremeZavrsetka - VremePocetka).TotalHours;
    }
}