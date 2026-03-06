using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public interface IRezervacijaServis
    {
        Rezervacija getRezervacija(int id);
        bool kreirajRezervaciju(Korisnik korisnik, Resurs resurs, Lokacija lokacija, DateTime pocetak, DateTime kraj);
        bool otkaziRezervaciju(int id);
        bool izmeniRezervaciju(Rezervacija rezervacija);
    }
}