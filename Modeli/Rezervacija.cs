namespace CoWorkingManager.Modeli
{
    // Predstavlja jednu rezervaciju resursa od strane korisnika
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
        public DateTime PocetakVreme { get; set; }

        // Datum i vreme završetka rezervacije
        public DateTime KrajVreme { get; set; }

        // Status rezervacije, cuva se kao string u bazi:
        // 'aktivna' | 'zavrsena' | 'otkazana'
        public StatusRezervacije StatusRezervacije { get; set; } = StatusRezervacije.Aktivna;

        // Trajanje rezervacije u satima (izracunato, nije u bazi)
        public double TrajanjeSati => (KrajVreme - PocetakVreme).TotalHours;
    }
}