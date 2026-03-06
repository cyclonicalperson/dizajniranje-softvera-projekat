using System;
using CoWorkingManager.Podaci;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public class LokacijaServisProxy
    {
        private readonly LokacijaServis _praviLokacijaServis;

        public LokacijaServisProxy(LokacijaServis praviLokacijaServis)
        {
            _praviLokacijaServis = praviLokacijaServis;
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

        public Lokacija getLokacija(int id)
        {
            proveriAdmina();
            return _praviLokacijaServis.getLokacija(id);
        }

        public bool dodajLokaciju(Lokacija lokacija)
        {
            proveriAdmina();
            return _praviLokacijaServis.dodajLokaciju(lokacija);
        }

        public bool obrisiLokaciju(int id)
        {
            proveriAdmina();
            return _praviLokacijaServis.obrisiLokaciju(id);
        }

        public bool izmeniLokaciju(Lokacija lokacija)
        {
            proveriAdmina();
            return _praviLokacijaServis.izmeniLokaciju(lokacija);
        }
    }
}