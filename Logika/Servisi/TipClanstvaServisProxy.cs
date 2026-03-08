using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoWorkingManager.Logika.Servisi
{
    public class TipClanstvaServisProxy
    {
        private readonly TipClanstvaServis _praviTipClanstvaServis;
        public TipClanstvaServisProxy(TipClanstvaServis praviTipClanstvaServis)
        {
            _praviTipClanstvaServis = praviTipClanstvaServis;
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

        public List<TipClanstva> dajSve()
        {
            proveriAdmina();
            return _praviTipClanstvaServis.dajSve();
        }

        public bool dodajTipClanstva(string ime, decimal cena, int trajanje, int maxSatiPoMesecu, bool pristupSali, int? brojSatiUSaliMesecno)
        {
            proveriAdmina();
            return _praviTipClanstvaServis.dodajTipClanstva(ime, cena, trajanje, maxSatiPoMesecu, pristupSali, brojSatiUSaliMesecno);
        }
    }
}
