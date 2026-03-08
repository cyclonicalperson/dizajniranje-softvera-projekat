using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Logika.Servisi
{
    public class TipClanstvaServis : BazniServis, ITipClanstvaServis
    {
        private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();

        public List<TipClanstva> dajSve()
        {
            var tipovi = _fasada.TipoviClanstva.DajSve();
            notifikacija("Dohvaceni svi tipovi clanstva");
            return tipovi;
        }

        public bool dodajTipClanstva(string ime, decimal cena, int trajanje, int maxSatiPoMesecu, bool pristupSali, int? brojSatiUSaliMesecno)
        {
            if (string.IsNullOrWhiteSpace(ime))
            {
                notifikacija("Naziv tipa clanstva je obavezno polje");
                return false;
            }

            if (cena <= 0)
            {
                notifikacija("Cena mora biti pozitivan broj");
                return false;
            }
            if (trajanje <= 0)
            {
                notifikacija("Trajanje mora biti pozitivan broj dana");
                return false;
            }
            if (maxSatiPoMesecu <= 0)
            {
                notifikacija("Maksimalan broj sati po mesecu mora biti pozitivan broj");
                return false;
            }

            if (brojSatiUSaliMesecno != null && !pristupSali)
            {
                notifikacija("Broj sati u sali mesecno moze biti postavljen samo ako tip clanstva ima pristup sali");
                return false;
            }

            if (brojSatiUSaliMesecno != null && brojSatiUSaliMesecno <= 0)
            {
                notifikacija("Broj sati u sali mesecno mora biti pozitivan broj");
                return false;
            }

            if (pristupSali && brojSatiUSaliMesecno != null && brojSatiUSaliMesecno > maxSatiPoMesecu)
            {
                notifikacija("Broj sati u sali mesecno ne moze biti veci od maksimalnog broja sati po mesecu");
                return false;
            }

            var tipClanstva = new TipClanstva
            {
                Ime = ime,
                Cena = cena,
                Trajanje = trajanje,
                MaxSatiPoMesecu = maxSatiPoMesecu,
                PristupSali = pristupSali,
                BrojSatiUSaliMesecno = brojSatiUSaliMesecno
            };

            if (_fasada.TipoviClanstva.Dodaj(tipClanstva))
            {
                notifikacija("Dodat novi tip clanstva");
                return true;
            }
            notifikacija("Dodavanje tipa clanstva neuspesno — naziv paketa vec postoji");
            return false;
        }
    }
}