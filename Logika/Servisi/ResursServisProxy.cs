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
        public Resurs getResurs(int id)
        {
            proveriAdmina();
            return _praviResursServis.getResurs(id);
        }

        public List<Resurs> dajSve()
        {
            proveriAdmina();
            return _praviResursServis.dajSve();
        }

        public bool kreirajResurs(string ime, string imeLokacije, string tipResursa,
            string? opis, string? podTipStola, int? kapacitet, bool? imaProjektor, bool? imaTV, bool? imaTablu, bool? imaOnlineOpremu)
        {
            proveriAdmina();
            return _praviResursServis.dodajResurs(ime, imeLokacije, tipResursa, opis, podTipStola, kapacitet, imaProjektor, imaTV, imaTablu, imaOnlineOpremu);
        }
        public bool otkaziResurs(string ime)
        {
            proveriAdmina();
            return _praviResursServis.obrisiResurs(ime);
        }
        public bool izmeniResurs(string ime, string? imeLokacije, string? tipResursa,
            string? opis, string? podTipStola, int? kapacitet, bool? imaProjektor, bool? imaTV, bool? imaTablu, bool? imaOnlineOpremu)
        {
            proveriAdmina();
            return _praviResursServis.izmeniResurs(ime, imeLokacije, tipResursa, opis, podTipStola, kapacitet, imaProjektor, imaTV, imaTablu, imaOnlineOpremu);
        }
        public List<Resurs> dajResursePoLokacijiSortiranoPoTipu(string? lokacija)
        {
            proveriAdmina();
            return _praviResursServis.dajResursePoLokacijiSortiranoPoTipu(lokacija);
        }
    }
}
