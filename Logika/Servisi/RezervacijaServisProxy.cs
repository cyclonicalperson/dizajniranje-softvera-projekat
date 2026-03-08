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
        Rezervacija getRezervacija(int id)
		{
            proveriAdmina();
            return _praviRezervacijaServis.getRezervacija(id);
		}
		bool kreirajRezervaciju(string ime, string prezime, string resursIme, string pocetak, string kraj)
		{
            proveriAdmina();   
            return _praviRezervacijaServis.kreirajRezervaciju(ime, prezime, resursIme, pocetak, kraj);
        }
		bool otkaziRezervaciju(string ime, string prezime, string resursIme, string pocetak, string kraj)
		{
            proveriAdmina();
            return _praviRezervacijaServis.otkaziRezervaciju(ime, prezime, resursIme, pocetak, kraj);
        }
        bool izmeniRezervaciju(string ime, string prezime, string? resursIme, string? pocetak, string? kraj)
		{
            proveriAdmina();
            return _praviRezervacijaServis.izmeniRezervaciju(ime, prezime, resursIme, pocetak, kraj);
        }
        public List<Rezervacija> dajRezervacijeKorisnika(Korisnik korisnik)
        {
            proveriAdmina();
            return _praviRezervacijaServis.dajRezervacijeKorisnika(korisnik);
        }
        public List<Rezervacija> dajRezervacijePoLokacijiIDanu(Lokacija lokacija, DateTime datum)
        {
            proveriAdmina();
            return _praviRezervacijaServis.dajRezervacijePoLokacijiIDanu(lokacija, datum);
        }
    }
}