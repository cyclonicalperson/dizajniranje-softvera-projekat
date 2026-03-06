using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Logika.Servisi
{
    public class KorisnikServis : BazniServis, IKorisnikServis
    {
        private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();

        public Korisnik getKorisnik(int id)
        {
            var korisnik = _fasada.Korisnici.DajPoId(id);
            if (korisnik == null) notifikacija("Korisnik je null");
            else notifikacija("Uzet korisnik");
            return korisnik;
        }

        public bool dodajKorisnika(Korisnik korisnik)
        {
            if(_fasada.Korisnici.Dodaj(korisnik))
            {
                notifikacija("Novi korisnik je dodat");
                return true;
            }
            notifikacija("Dodavanje novog korisnika neuspesno");
            return false;
        }

        public bool obrisiKorisnika(int id)
        {
            if(_fasada.Korisnici.Obrisi(id))
            {
                notifikacija("Obrisan korisnik");
                return true;
            }
            notifikacija("Brisanje korisnika neuspesno");
            return false;
        }

        public bool izmeniKorisnika(Korisnik korisnik)
        {
            if(_fasada.Korisnici.Azuriraj(korisnik))
            {
                notifikacija("Izmenjen korisnik");
                return true;
            }
            notifikacija("Izmene korisnika neuspesne");
            return false;
        }
    }
}
