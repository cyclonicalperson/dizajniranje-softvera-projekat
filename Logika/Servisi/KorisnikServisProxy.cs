using CoWorkingManager.Podaci;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public class KorisnikServisProxy : IKorisnikServis
    {
        private readonly KorisnikServis _praviKorisnikServis;

        public KorisnikServisProxy(KorisnikServis praviKorisnikServis)
        {
            _praviKorisnikServis = praviKorisnikServis;
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

        public Korisnik getKorisnik(int id)
        {
            proveriAdmina();
            return _praviKorisnikServis.getKorisnik(id);
        }

        public bool dodajKorisnika(Korisnik korisnik)
        {
            proveriAdmina();
            return _praviKorisnikServis.dodajKorisnika(korisnik);
        }

        public bool obrisiKorisnika(int id)
        {
            proveriAdmina();
            return _praviKorisnikServis.obrisiKorisnika(id);
        }

        public bool izmeniKorisnika(Korisnik korisnik)
        {
            proveriAdmina();
            return _praviKorisnikServis.izmeniKorisnika(korisnik);
        }
    }
}
