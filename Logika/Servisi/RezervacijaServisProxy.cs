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
		bool kreirajRezervaciju(Korisnik korisnik, Resurs resurs, Lokacija lokacija, DateTime pocetak, DateTime kraj)
		{
            proveriAdmina();   
            return _praviRezervacijaServis.kreirajRezervaciju(korisnik, resurs, lokacija, pocetak, kraj);
        }
		bool otkaziRezervaciju(int id)
		{
            proveriAdmina();
            return _praviRezervacijaServis.otkaziRezervaciju(id);
        }
        bool izmeniRezervaciju(Rezervacija rezervacija)
		{
            proveriAdmina();
            return _praviRezervacijaServis.izmeniRezervaciju(rezervacija);
        }
    }
}