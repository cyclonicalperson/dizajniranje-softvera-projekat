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

        public bool dodajKorisnika(string ime, string prezime, string email, string? telefon,
            int tipClanstvaId, string datumPocetkaClanstva, string datumKrajaClanstva,
            string statusNaloga)
        {
            proveriAdmina();
            return _praviKorisnikServis.dodajKorisnika(ime, prezime, email, telefon, tipClanstvaId, datumPocetkaClanstva, datumKrajaClanstva, statusNaloga);
        }

        public bool obrisiKorisnika(string ime, string prezime)
        {
            proveriAdmina();
            return _praviKorisnikServis.obrisiKorisnika(ime, prezime);
        }

        public bool izmeniKorisnika(string ime, string prezime, string? noviEmail, string? noviTelefon, int? noviTipClanstvaId, 
            string? noviDatumPocetkaClanstva, string? noviDatumKrajaClanstva, string? noviStatusNaloga)
        {
            proveriAdmina();
            return _praviKorisnikServis.izmeniKorisnika(ime, prezime, noviEmail, noviTelefon, noviTipClanstvaId, noviDatumPocetkaClanstva, noviDatumKrajaClanstva, noviStatusNaloga);
        }
    }
}
