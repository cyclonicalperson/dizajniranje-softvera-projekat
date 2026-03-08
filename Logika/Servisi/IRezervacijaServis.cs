using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public interface IRezervacijaServis
    {
        Rezervacija getRezervacija(int id);
        List<Rezervacija> dajSve();
        bool kreirajRezervaciju(Korisnik korisnik, Resurs resurs, DateOnly? pocetakDatum, string? pocetakVreme, DateOnly? krajDatum, string? krajVreme);
        bool otkaziRezervaciju(Korisnik korisnik, Resurs resurs);
        bool izmeniRezervaciju(Korisnik korisnik, Resurs resurs, DateOnly? pocetakDatum, string? pocetakVreme, DateOnly? krajDatum, string? krajVreme);
        List<Rezervacija> dajRezervacijeKorisnika(string? imePrezime);
        List<Rezervacija> dajRezervacijePoLokacijiIDanu(string? lokacija, DateTime? datum);
    }
}