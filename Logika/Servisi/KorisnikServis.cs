using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using System.Windows.Media;

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
        // Vraca filtrirani spisak korisnika po lokaciji, tipu clanstva i statusu naloga
        // Svi parametri su opcioni — prosleđuje null za parametre koji se ne filtriraju
        public List<Korisnik> dajKorisnike(string? lokacija, string? tipClanstva, string? statusNaloga)
        {
            int? lokacijaId = null;
            if (!string.IsNullOrEmpty(lokacija))
            {
                var l = _fasada.Lokacije.DajSve().FirstOrDefault(x => x.Ime == lokacija);
                if (l != null)
                    lokacijaId = l.Id;
            }

            int? tipClanstvaId = null;
            if (!string.IsNullOrEmpty(tipClanstva))
            {
                var t = _fasada.TipoviClanstva.DajSve().FirstOrDefault(x => x.Ime == tipClanstva);
                if (t != null)
                    tipClanstvaId = t.Id;
            }

            StatusNaloga status = StatusNaloga.Aktivan; // default
            if (!string.IsNullOrEmpty(statusNaloga))
            {
                Enum.TryParse(statusNaloga, out status);
            }

            var korisnici = _fasada.Korisnici.DajPoFiltru(lokacijaId, tipClanstvaId, status);
            notifikacija("Dohvacena lista korisnika");
            return korisnici;
        }

        public List<string> dajStatuseNaloga()
        {
            List<string> statusi = new List<string>();

            foreach (var korisnik in _fasada.Korisnici.DajSve())
            {
                statusi.Add(korisnik.StatusNaloga.ToString());
            }

            notifikacija("Dohvaceni statusi naloga");
            return statusi;
        }
    }
}
