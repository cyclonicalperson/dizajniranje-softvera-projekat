using CoWorkingManager.Podaci;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public class KorisnikServisProxy
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

        public List<Korisnik> dajSve()
        {
            proveriAdmina();
            return _praviKorisnikServis.dajSve();
        }

        public bool dodajKorisnika(string ime, string prezime, string email, string? telefon,
            string tipClanstva, DateOnly datumPocetkaClanstva, DateOnly datumKrajaClanstva,
            string statusNaloga)
        {
            proveriAdmina();
            return _praviKorisnikServis.dodajKorisnika(ime, prezime, email, telefon, tipClanstva, datumPocetkaClanstva, datumKrajaClanstva, statusNaloga);
        }

        public bool obrisiKorisnika(string ime, string prezime)
        {
            proveriAdmina();
            return _praviKorisnikServis.obrisiKorisnika(ime, prezime);
        }

        public bool izmeniKorisnika(string ime, string prezime, string? noviEmail, string? noviTelefon, string? noviTipClanstva,
            DateOnly? noviDatumPocetkaClanstva, DateOnly? noviDatumKrajaClanstva, string? noviStatusNaloga)
        {
            proveriAdmina();
            return _praviKorisnikServis.izmeniKorisnika(ime, prezime, noviEmail, noviTelefon, noviTipClanstva, noviDatumPocetkaClanstva, noviDatumKrajaClanstva, noviStatusNaloga);
        }
        public List<Korisnik> dajKorisnike(string? lokacija, string? tipClanstva, string? statusNaloga)
        {
            proveriAdmina();
            return _praviKorisnikServis.dajKorisnike(lokacija, tipClanstva, statusNaloga);
        }
        public List<string> dajStatuseNaloga()
        {
            proveriAdmina();
            return _praviKorisnikServis.dajStatuseNaloga();
        }
    }
}
