using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Logika.Servisi
{
    public class RezervacijaServis : BazniServis, IRezervacijaServis
    {
        private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();
        public Rezervacija getRezervacija(int id)
        {
            var rezervacija = _fasada.Rezervacije.DajPoId(id);
            if (rezervacija == null) notifikacija("Rezervacija je null");
            else notifikacija("Uzeta rezervacija");
            return rezervacija;
        }
        public bool kreirajRezervaciju(Korisnik korisnik, Resurs resurs, Lokacija lokacija, DateTime pocetak, DateTime kraj)
        {
            Rezervacija rezervacija = new Rezervacija();
            rezervacija.Korisnik = korisnik;
            rezervacija.Resurs = resurs;
            rezervacija.PocetakVreme = pocetak;
            rezervacija.KrajVreme = kraj;
            if (!validacijaClanstva(korisnik, resurs, pocetak, kraj)) return false;
            if (!validacijaTermina(resurs, pocetak, kraj)) return false;
            if (!validacijaRadnogVremena(lokacija, pocetak, kraj)) return false;
            if (_fasada.Rezervacije.Dodaj(rezervacija))
            {
                notifikacija("Nova rezervacija je kreirana");
                return true;
            }
            notifikacija("Kreiranje nove rezervacije neuspesno");
            return false;
        }
        public bool otkaziRezervaciju(int id)
        {
            if (_fasada.Rezervacije.Otkazi(id))
            {
                notifikacija("Otkazana rezervacija");
                return true;
            }
            notifikacija("Otkazivanje rezervacija neuspesno");
            return false;
        }
        public bool izmeniRezervaciju(Rezervacija rezervacija)
        {
            if (_fasada.Rezervacije.Azuriraj(rezervacija))
            {
                notifikacija("Izmenjena rezervacija");
                return true;
            }
            notifikacija("Izmene rezervacije neuspesna");
            return false;
        }
        private bool validacijaClanstva(Korisnik korisnik, Resurs resurs, DateTime pocetak, DateTime kraj)
        {
            if (DateOnly.FromDateTime(pocetak) < korisnik.DatumPocetkaClanstva || DateOnly.FromDateTime(kraj) > korisnik.DatumKrajaClanstva)
                return false;

            int vremeClan;

            if (resurs.TipResursa == TipResursa.Sala)
                vremeClan = (int)korisnik.TipClanstva.BrojSatiUSaliMesecno;
            else
                vremeClan = korisnik.TipClanstva.MaxSatiPoMesecu;

            if (vremeClan != 0)
            {
                foreach (Rezervacija r in _fasada.Rezervacije.DajZaMesecniIzvestaj(pocetak.Year, pocetak.Month))
                {
                    if (r.Korisnik.Id != korisnik.Id)
                        continue;

                    if (resurs.TipResursa == TipResursa.Sala && r.Resurs.TipResursa != TipResursa.Sala)
                        continue;

                    if (resurs.TipResursa != TipResursa.Sala && r.Resurs.TipResursa == TipResursa.Sala)
                        continue;

                    vremeClan -= (int)r.TrajanjeSati;
                }
            }

            int trajanje = (int)(kraj - pocetak).TotalHours;
            vremeClan -= trajanje;

            if (vremeClan < 0)
                return false;

            return true;
        }
        private bool validacijaTermina(Resurs resurs, DateTime pocetak, DateTime kraj)
        {
            return !_fasada.Rezervacije.PostojiPreklapanje(resurs.Id, pocetak, kraj);
        }
        private bool validacijaRadnogVremena(Lokacija lokacija, DateTime pocetakRez, DateTime krajRez)
        {
            if (krajRez <= pocetakRez)
            {
                throw new Exception("Kraj rezervacije mora biti posle početka.");
            }
            string[] parts = lokacija.RadniSati.Split('-');

            TimeSpan pocetakRadnog = TimeSpan.Parse(parts[0].Trim());
            TimeSpan krajRadnog = TimeSpan.Parse(parts[1].Trim());
            TimeSpan pocetak = pocetakRez.TimeOfDay;
            TimeSpan kraj = krajRez.TimeOfDay;
            return pocetak >= pocetakRadnog && kraj <= krajRadnog;
        }
        // Vraca sve rezervacije prosleđenog korisnika, sortirane od najnovije
        public List<Rezervacija> dajRezervacijeKorisnika(Korisnik korisnik)
        {
            var rezervacije = _fasada.Rezervacije.DajPoKorisniku(korisnik.Id);
            notifikacija("Dohvacene rezervacije korisnika");
            return rezervacije;
        }
        // Vraca sve rezervacije na datoj lokaciji za dati dan, sortirane po vremenu pocetka
        // Ne ukljucuje otkazane rezervacije
        public List<Rezervacija> dajRezervacijePoLokacijiIDanu(Lokacija lokacija, DateTime datum)
        {
            var rezervacije = _fasada.Rezervacije.DajPoLokacijiIDanu(lokacija.Id, datum);
            notifikacija("Dohvacene rezervacije po lokaciji i danu");
            return rezervacije;
        }
    }
}