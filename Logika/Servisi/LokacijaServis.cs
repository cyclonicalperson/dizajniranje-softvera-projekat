using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using CoWorkingManager.Podaci.Repozitorijumi;

namespace CoWorkingManager.Logika.Servisi
{
    public class LokacijaServis : BazniServis, ILokacijaServis
    {
        private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();

        public Lokacija getLokacija(int id)
        {
            var lokacija = _fasada.Lokacije.DajPoId(id);
            if (lokacija == null) notifikacija("Lokacija je null");
            else notifikacija("Uzeta lokacija");
            return lokacija;
        }

        public List<Lokacija> dajSve()
        {
            var lokacije = _fasada.Lokacije.DajSve();
            notifikacija("Dohvacene sve lokacije");
            return lokacije;
        }


        public bool dodajLokaciju(string ime, string adresa, string grad, string radniSati, int maxBrojKorisnika)
        {
            var lokacija = new Lokacija
            {
                Ime = ime,
                Adresa = adresa,
                Grad = grad,
                RadniSati = radniSati,
                MaxBrojKorisnika = maxBrojKorisnika
            };
            if (_fasada.Lokacije.Dodaj(lokacija))
            {
                notifikacija("Nova lokacija je dodata");
                return true;
            }
            notifikacija("Dodavanje nove lokacije neuspesno");
            return false;
        }

        public bool obrisiLokaciju(string ime)
        {
            var lokacija = _fasada.Lokacije.DajPoNazivu(ime);
            if (lokacija == null)
            {
                notifikacija("Brisanje lokacije neuspesno jer lokacija nije pronadjena");
                return false;
            }

            if (_fasada.Lokacije.Obrisi(lokacija.Id))
            {
                notifikacija("Obrisana lokacija");
                return true;
            }
            notifikacija("Brisanje lokacije neuspesno");
            return false;
        }

        public bool izmeniLokaciju(string ime, string? adresa, string? grad, string? radniSati, int? maxBrojKorisnika)
        {
            var lokacija = _fasada.Lokacije.DajPoNazivu(ime);
            if (lokacija == null)
            {
                notifikacija("Izmena lokacije neuspesna jer lokacija nije pronadjena");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(adresa)) lokacija.Adresa = adresa;
            if (!string.IsNullOrWhiteSpace(grad)) lokacija.Grad = grad;
            if (!string.IsNullOrWhiteSpace(radniSati)) lokacija.RadniSati = radniSati;
            if (maxBrojKorisnika != null) lokacija.MaxBrojKorisnika = maxBrojKorisnika.Value;
            if (_fasada.Lokacije.Azuriraj(lokacija))
            {
                notifikacija("Izmenjena lokacija");
                return true;
            }
            notifikacija("Izmena lokacije neuspesna");
            return false;
        }

        public Lokacija pronadjiLokaciju(string ime)
        {
            var lokacija = _fasada.Lokacije.DajPoNazivu(ime);
            if (lokacija == null) notifikacija("Lokacija nije pronadjena");
            else notifikacija("Uzeta lokacija");
            return lokacija;
        }

        public List<StatistikaZauzetosti> dajStatistikuZauzetostiZaSve(DateTime uTrenutku)
        {
            var statistika = _fasada.Lokacije.DajStatistikuZauzetostiZaSve(uTrenutku);
            notifikacija("Dohvacena statistika zauzetosti za sve lokacije");
            return statistika;
        }
    }
}