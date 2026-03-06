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
        bool dodajTipClanstva(TipClanstva tipClanstva)
        {
            proveriAdmina();
            return _praviTipClanstvaServis.dodajTipClanstva(tipClanstva);
        }
    }
}
