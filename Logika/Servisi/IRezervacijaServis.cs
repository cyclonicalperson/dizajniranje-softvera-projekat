using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public interface IRezervacijaServis
    {
        Rezervacija getRezervacija(int id);
        bool kreirajRezervaciju(string ime, string prezime, string resursIme, string pocetak, string kraj);
        bool otkaziRezervaciju(string ime, string prezime, string resursIme, string pocetak, string kraj);
        bool izmeniRezervaciju(string ime, string prezime, string? resursIme, string? pocetak, string? kraj);
    }
}