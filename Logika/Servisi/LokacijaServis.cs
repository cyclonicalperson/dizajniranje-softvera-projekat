using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

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

        public bool dodajLokaciju(Lokacija lokacija)
        {
            if(_fasada.Lokacije.Dodaj(lokacija))
            {
                notifikacija("Nova lokacija je dodata");
                return true;
            }
            notifikacija("Dodavanje nove lokacije neuspesno");
            return false;
        }

        public bool obrisiLokaciju(int id)
        {
            if(_fasada.Lokacije.Obrisi(id))
            {
                notifikacija("Obrisana lokacija");
                return true;
            }
            notifikacija("Brisanje lokacije neuspesno");
            return false;
        }

        public bool izmeniLokaciju(Lokacija lokacija)
        {
            if(_fasada.Lokacije.Azuriraj(lokacija))
            {
                notifikacija("Izmenjena lokacija");
                return true;
            }
            notifikacija("Izmena lokacije neuspesna");
            return false;
        }
    }
}