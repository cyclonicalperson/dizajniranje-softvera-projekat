using CoWorkingManager.Podaci;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public class RezervacijaServisProxy
    {
        private readonly RezervacijaServis _praviRezervacijaServis;
        public RezervacijaServisProxy(RezervacijaServis praviRezervacijaServis)
        {
            _praviRezervacijaServis = praviRezervacijaServis;
        }
        public bool autentifikacija(string korisnickoIme, string lozinka)
        {
            Administrator admin = CoworkingFasada.LoginAdmin(korisnickoIme, lozinka);
            if (admin != null)
            {
                AdminSession.Instance.Login(admin);
                return true;
            }
            return false;
        }
        private void proveriAdmina()
        {
            if (AdminSession.Instance.Admin == null)
                throw new UnauthorizedAccessException("Administrator nije prijavljen.");
        }
        public Rezervacija getRezervacija(int id)
        {
            proveriAdmina();
            return _praviRezervacijaServis.getRezervacija(id);
        }

        public List<Rezervacija> dajSve()
        {
            proveriAdmina();
            return _praviRezervacijaServis.dajSve();
        }

        public bool kreirajRezervaciju(string ime, string prezime, string resursIme, string pocetak, string kraj)
        {
            proveriAdmina();
            return _praviRezervacijaServis.kreirajRezervaciju(ime, prezime, resursIme, pocetak, kraj);
        }
        public bool otkaziRezervaciju(Korisnik korisnik, Resurs resurs)
        {
            proveriAdmina();
            return _praviRezervacijaServis.otkaziRezervaciju(korisnik, resurs);
        }
        public bool izmeniRezervaciju(Korisnik korisnik, Resurs resurs, DateTime? pocetak, DateTime? kraj)
        {
            proveriAdmina();
            return _praviRezervacijaServis.izmeniRezervaciju(korisnik, resurs, pocetak, kraj);
        }
        public List<Rezervacija> dajRezervacijeKorisnika(string? korisnik)
        {
            proveriAdmina();
            return _praviRezervacijaServis.dajRezervacijeKorisnika(korisnik);
        }
        public List<Rezervacija> dajRezervacijePoLokacijiIDanu(string? lokacija, DateTime? datum)
        {
            proveriAdmina();
            return _praviRezervacijaServis.dajRezervacijePoLokacijiIDanu(lokacija, datum);
        }
    }
}