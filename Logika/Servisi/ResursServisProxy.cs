using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoWorkingManager.Logika.Servisi
{
    public class ResursServisProxy
    {
        private readonly ResursServis _praviResursServis;
        public ResursServisProxy(ResursServis praviResursServis)
        {
            _praviResursServis = praviResursServis;
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
        Resurs getResurs(int id)
        {
            proveriAdmina();
            return _praviResursServis.getResurs(id);
        }
        bool kreirajResurs(Resurs resurs)
        {
            proveriAdmina();
            return _praviResursServis.dodajResurs(resurs);
        }
        bool otkaziResurs(int id)
        {
            proveriAdmina();
            return _praviResursServis.obrisiResurs(id);
        }
        bool izmeniResurs(Resurs resurs)
        {
            proveriAdmina();
            return _praviResursServis.izmeniResurs(resurs);
        }
    }
}
