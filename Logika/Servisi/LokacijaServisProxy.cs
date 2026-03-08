using System;
using CoWorkingManager.Podaci;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci.Repozitorijumi;

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

        public List<Lokacija> dajSve()
        {
            proveriAdmina();
            return _praviLokacijaServis.dajSve();
        }

        public bool dodajLokaciju(string ime, string adresa, string grad, string radniSati, int maxBrojKorisnika)
        {
            proveriAdmina();
            return _praviLokacijaServis.dodajLokaciju(ime, adresa, grad, radniSati, maxBrojKorisnika);
        }

        public bool obrisiLokaciju(string ime)
        {
            proveriAdmina();
            return _praviLokacijaServis.obrisiLokaciju(ime);
        }

        public bool izmeniLokaciju(string ime, string? adresa, string? grad, string? radniSati, int? maxBrojKorisnika)
        {
            proveriAdmina();
            return _praviLokacijaServis.izmeniLokaciju(ime, adresa, grad, radniSati, maxBrojKorisnika);
        }
        public Lokacija pronadjiLokaciju(string ime)
        {
            proveriAdmina();
            return _praviLokacijaServis.pronadjiLokaciju(ime);
        }

        public List<StatistikaZauzetosti> dajStatistikuZauzetostiZaSve(DateTime uTrenutku)
        {
            proveriAdmina();
            return _praviLokacijaServis.dajStatistikuZauzetostiZaSve(uTrenutku);
        }
    }
}