using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public List<Rezervacija> dajSve()
        {
            var rezervacije = _fasada.Rezervacije.DajSve();
            notifikacija("Dohvacene sve rezervacije");
            return rezervacije;
        }

        public bool kreirajRezervaciju(Korisnik korisnik, Resurs resurs, DateOnly? pocetakDatum, string? pocetakVreme, DateOnly? krajDatum, string? krajVreme)
        {
            DateTime? pocetak = null;
            if (pocetakDatum != null && pocetakVreme != null)
            {
                DateOnly d = pocetakDatum.Value;
                TimeOnly t = TimeOnly.Parse(pocetakVreme);
                pocetak = d.ToDateTime(t);
            }
            DateTime? kraj = null;
            if (krajDatum != null && krajVreme != null)
            {
                DateOnly d = krajDatum.Value;
                TimeOnly t = TimeOnly.Parse(krajVreme);
                kraj = d.ToDateTime(t);
            }
            if (pocetak == null || kraj == null)
            {
                notifikacija("Kreiranje rezervacije neuspesno — pocetak i kraj ne smeju biti null");
                return false;
            }
            if (pocetak >= kraj)
            {
                notifikacija("Kraj rezervacije mora biti posle početka.");
                return false;
            }

            if (pocetak.Value.Date != kraj.Value.Date)
            {
                notifikacija("Nije moguće imati rezervaciju koja ne traje u istom danu.");
                return false;
            }
            var lokacija = _fasada.Lokacije.DajPoId(resurs.LokacijaId);
            if (lokacija == null)
            {
                notifikacija("Kreiranje rezervacije neuspesno — lokacija resursa nije pronadjena");
                return false;
            }

            if (!validacijaClanstva(korisnik, resurs, pocetak.Value, kraj.Value))
            {
                notifikacija("Kreiranje rezervacije neuspesno — nije prosla validacija clanstva");
                return false;
            }
            if (!validacijaTermina(resurs, pocetak.Value, kraj.Value))
            {
                notifikacija("Kreiranje rezervacije neuspesno — termin nije slobodan");
                return false;
            }
            if (!validacijaRadnogVremena(lokacija, pocetak.Value, kraj.Value))
            {
                notifikacija("Kreiranje rezervacije neuspesno — nije u radnom vremenu lokacije");
                return false;
            }

            Rezervacija rezervacija = new RezervacijaBuilder()
                .ZaKorisnika(korisnik.Id)
                .NaResursu(resurs.Id)
                .Od(pocetak.Value)
                .Do(kraj.Value)
                .SaStatusom(StatusRezervacije.Aktivna)
                .Build();

            if (_fasada.Rezervacije.Dodaj(rezervacija))
            {
                notifikacija("Nova rezervacija je kreirana");
                return true;
            }
            notifikacija("Kreiranje nove rezervacije neuspesno");
            return false;
        }
        public bool otkaziRezervaciju(Korisnik korisnik, Resurs resurs)
        {
            var rezervacija = _fasada.Rezervacije.DajSve()
                .FirstOrDefault(r => r.Korisnik.Id == korisnik.Id && r.Resurs.Id == resurs.Id && r.StatusRezervacije == StatusRezervacije.Aktivna);
            if (rezervacija == null)
            {
                notifikacija("Rezervacija nije pronadjena");
                return false;
            }
            if (_fasada.Rezervacije.Otkazi(rezervacija.Id))
            {
                notifikacija("Otkazana rezervacija");
                return true;
            }
            notifikacija("Otkazivanje rezervacija neuspesno");
            return false;
        }
        public bool izmeniRezervaciju(Korisnik korisnik, Resurs resurs, DateOnly? pocetakDatum, string? pocetakVreme, DateOnly? krajDatum, string? krajVreme)
        {
            var rezervacija = _fasada.Rezervacije.DajSve()
                .FirstOrDefault(r => r.Korisnik.Id == korisnik.Id && r.Resurs.Id == resurs.Id && r.StatusRezervacije == StatusRezervacije.Aktivna);
            if (rezervacija == null)
            {
                notifikacija("Rezervacija nije pronadjena");
                return false;
            }
            if (pocetakDatum == null && pocetakVreme != null)
            {
                notifikacija("Ne mozete menjati vreme pocetka bez menjanja datuma");
                return false;
            }
            if (krajDatum == null && krajVreme != null)
            {
                notifikacija("Ne mozete menjati vreme kraja bez menjanja datuma");
                return false;
            }
            if (pocetakDatum != null && pocetakVreme == null)
            {
                notifikacija("Ne mozete menjati datum pocetka bez menjanja vremena");
                return false;
            }
            if (krajDatum != null && krajVreme == null)
            {
                notifikacija("Ne mozete menjati datum kraja bez menjanja vremena");
                return false;
            }
            DateTime? pocetak = null;
            if (pocetakDatum != null && pocetakVreme != null)
            {
                DateOnly d = pocetakDatum.Value;
                TimeOnly t = TimeOnly.Parse(pocetakVreme);
                pocetak = d.ToDateTime(t);
            }
            DateTime? kraj = null;
            if (krajDatum != null && krajVreme != null)
            {
                DateOnly d = krajDatum.Value;
                TimeOnly t = TimeOnly.Parse(krajVreme);
                kraj = d.ToDateTime(t);
            }
            if (pocetak == null) pocetak = rezervacija.PocetakVreme;
            if (kraj == null) kraj = rezervacija.KrajVreme;
            if (pocetak >= kraj)
            {
                notifikacija("Kraj rezervacije mora biti posle početka.");
                return false;
            }
            if (pocetak.Value.Date != kraj.Value.Date)
            {
                notifikacija("Nije moguće imati rezervaciju koja ne traje u istom danu.");
                return false;
            }
            if (!validacijaClanstva(korisnik, resurs, (DateTime)pocetak, (DateTime)kraj))
            { 
                notifikacija("Nije prosla validacija clanstva");
                return false; 
            }
            if (!validacijaTermina(resurs, (DateTime)pocetak, (DateTime)kraj))
            { 
                notifikacija("Termin nije slobodan");
                return false; 
            }
            var lokacijaObj = _fasada.Lokacije.DajPoId(resurs.LokacijaId);
            if (lokacijaObj == null)
            {
                notifikacija("Lokacija nije pronadjena");
                return false;
            }
            if (!validacijaRadnogVremena(lokacijaObj, (DateTime)pocetak, (DateTime)kraj)) 
            {
                notifikacija("Nije u radnom vremenu lokacije");
                return false; 
            }
            if (pocetak != null)
                rezervacija.PocetakVreme = (DateTime)pocetak;
            if (kraj != null)
                rezervacija.KrajVreme = (DateTime)kraj;
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
        public List<Rezervacija> dajRezervacijeKorisnika(string? imePrezime)
        {
            var rezervacije = new List<Rezervacija>();
            if (imePrezime == null)
            {
                rezervacije = _fasada.Rezervacije.DajSve().OrderByDescending(r => r.PocetakVreme).ToList();
                return rezervacije;
            }
            var ime = imePrezime.Split(' ')[0];
            var prezime = imePrezime.Split(' ')[1];
            var korisnik = _fasada.Korisnici.DajSve()
                .FirstOrDefault(k => k.Ime == ime && k.Prezime == prezime);
            if (korisnik == null) 
            {
                notifikacija("Korisnik nije pronadjen");
                return new List<Rezervacija>();
            }
            rezervacije = _fasada.Rezervacije.DajPoKorisniku(korisnik.Id);
            notifikacija("Dohvacene rezervacije korisnika");
            return rezervacije;
        }

        // Ne ukljucuje otkazane rezervacije
        public List<Rezervacija> dajRezervacijePoLokacijiIDanu(string? lokacija, DateTime? datum)
        {

            if (lokacija != null && datum != null)
            {
                var lokacijaObj = _fasada.Lokacije.DajPoNazivu(lokacija);
                if (lokacijaObj == null)
                {
                    notifikacija("Lokacija nije pronadjena");
                    return new List<Rezervacija>();
                }
                return _fasada.Rezervacije.DajPoLokacijiIDanu(lokacijaObj.Id, datum.Value);
            }

            IEnumerable<Rezervacija> sve = _fasada.Rezervacije.DajSve();

            if (lokacija != null)
            {
                var lokacijaObj = _fasada.Lokacije.DajPoNazivu(lokacija);

                if (lokacijaObj == null)
                {
                    notifikacija("Lokacija nije pronadjena");
                    return new List<Rezervacija>();
                }

                sve = sve.Where(r => r.Resurs.LokacijaId == lokacijaObj.Id);
            }

            if (datum != null)
            {
                var pocetakDana = datum.Value.Date;
                var krajDana = pocetakDana.AddDays(1);

                sve = sve.Where(r => r.PocetakVreme < krajDana && r.KrajVreme > pocetakDana);
            }

            var rezervacije = sve.OrderBy(r => r.PocetakVreme).ToList();
            notifikacija("Dohvacene rezervacije po lokaciji i danu");
            return rezervacije;
        }
    }
}