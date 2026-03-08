using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public interface IRezervacijaServis
    {
        Rezervacija getRezervacija(int id);
        List<Rezervacija> dajSve();
        bool kreirajRezervaciju(string ime, string prezime, string resursIme, string pocetak, string kraj);
        bool otkaziRezervaciju(Korisnik korisnik, Resurs resurs);
        bool izmeniRezervaciju(Korisnik korisnik, Resurs resurs, DateTime? pocetak, DateTime? kraj);
        List<Rezervacija> dajRezervacijeKorisnika(string? imePrezime);
        List<Rezervacija> dajRezervacijePoLokacijiIDanu(string? lokacija, DateTime? datum);
    }
}