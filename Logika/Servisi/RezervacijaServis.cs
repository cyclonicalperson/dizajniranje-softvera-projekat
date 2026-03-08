using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using System.Windows.Controls;

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

        public bool kreirajRezervaciju(string ime, string prezime, string resursIme, string pocetak, string kraj)
        {
            var korisnik = _fasada.Korisnici.DajSve()
                .FirstOrDefault(k => k.Ime == ime && k.Prezime == prezime);
            if (korisnik == null)
            {
                notifikacija("Korisnik nije pronadjen");
                return false;
            }
            var lokacijaObj = _fasada.Lokacije.DajPoId(_fasada.Resursi.DajSve().FirstOrDefault(r => r.Ime == resursIme)?.LokacijaId ?? -1);
            if (lokacijaObj == null)
            {
                notifikacija("Lokacija nije pronadjena");
                return false;
            }
            var resurs = _fasada.Resursi.DajSve()
                .FirstOrDefault(r => r.Ime == resursIme && r.LokacijaId == lokacijaObj.Id);
            if (resurs == null)
            {
                notifikacija("Resurs nije pronadjen");
                return false;
            }
            DateTime pocetakDt, krajDt;
            pocetakDt = DateTime.Parse(pocetak);
            krajDt = DateTime.Parse(kraj);
            Rezervacija rezervacija = new Rezervacija();
            rezervacija.Korisnik = korisnik;
            rezervacija.Resurs = resurs;
            rezervacija.PocetakVreme = pocetakDt;
            rezervacija.KrajVreme = krajDt;
            if (!validacijaClanstva(korisnik, resurs, pocetakDt, krajDt)) return false;
            if (!validacijaTermina(resurs, pocetakDt, krajDt)) return false;
            if (!validacijaRadnogVremena(lokacijaObj, pocetakDt, krajDt)) return false;
            if (_fasada.Rezervacije.Dodaj(rezervacija))
            {
                notifikacija("Nova rezervacija je kreirana");
                return true;
            }
            notifikacija("Kreiranje nove rezervacije neuspesno");
            return false;
        }
        public bool otkaziRezervaciju(string ime, string prezime, string resursIme, string pocetak, string kraj)
        {
            var korisnik = _fasada.Korisnici.DajSve()
                .FirstOrDefault(k => k.Ime == ime && k.Prezime == prezime);
            if (korisnik == null)
            {
                notifikacija("Korisnik nije pronadjen");
                return false;
            }
            var resurs = _fasada.Resursi.DajSve()
                .FirstOrDefault(r => r.Ime == resursIme);
            if (resurs == null)
            {
                notifikacija("Resurs nije pronadjen");
                return false;
            }
            var rezervacija = _fasada.Rezervacije.DajSve()
                .FirstOrDefault(r => r.Korisnik.Id == korisnik.Id && r.Resurs.Id == resurs.Id
                    && r.PocetakVreme == DateTime.Parse(pocetak) && r.KrajVreme == DateTime.Parse(kraj));
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
        public bool izmeniRezervaciju(string ime, string prezime, string? resursIme, string? pocetak, string? kraj)
        {
            var korisnik = _fasada.Korisnici.DajSve()
                .FirstOrDefault(k => k.Ime == ime && k.Prezime == prezime);
            if (korisnik == null)
            {
                notifikacija("Korisnik nije pronadjen");
                return false;
            }
            var rezervacija = _fasada.Rezervacije.DajSve()
                .FirstOrDefault(r => r.Korisnik.Id == korisnik.Id && r.StatusRezervacije == StatusRezervacije.Aktivna);
            if (rezervacija == null)
            {
                notifikacija("Rezervacija nije pronadjena");
                return false;
            }
            var resurs = new Resurs();
            var lokacijaObj = new Lokacija();
            if (!string.IsNullOrWhiteSpace(resursIme))
            {
                resurs = _fasada.Resursi.DajSve()
                .FirstOrDefault(r => r.Ime == resursIme);
                if (resurs == null)
                {
                    notifikacija("Resurs nije pronadjen");
                    return false;
                }
                lokacijaObj = _fasada.Lokacije.DajPoId(resurs.LokacijaId);
                if (lokacijaObj == null)
                {
                    notifikacija("Lokacija nije pronadjena");
                    return false;
                }
            }
            else
            {
                resurs = rezervacija.Resurs;
                lokacijaObj = _fasada.Lokacije.DajPoId(resurs.LokacijaId);
                if (lokacijaObj == null)
                {
                    notifikacija("Lokacija nije pronadjena");
                    return false;
                }
            }
            var pocetakDt = new DateTime();
            if (pocetak != null)
            {
                pocetakDt = DateTime.Parse(pocetak);
            }
            else
            {
                pocetakDt = rezervacija.PocetakVreme;
            }
            var krajDt = new DateTime();
            if (kraj != null)
            {
                krajDt = DateTime.Parse(kraj);
            }
            else
            {
                krajDt = rezervacija.KrajVreme;
            }
            if (!validacijaClanstva(korisnik, resurs, pocetakDt, krajDt)) return false;
            if (!validacijaTermina(resurs, pocetakDt, krajDt)) return false;
            if (!validacijaRadnogVremena(lokacijaObj, pocetakDt, krajDt)) return false;
            if (resursIme != null)
            {
                rezervacija.Resurs = resurs;
                rezervacija.ResursId = resurs.Id;
            }
            if (pocetak != null)
                rezervacija.PocetakVreme = DateTime.Parse(pocetak);
            if (kraj != null)
                rezervacija.KrajVreme = DateTime.Parse(kraj);
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

            var sve = _fasada.Rezervacije.DajSve()
                .Where(r => r.StatusRezervacije != StatusRezervacije.Otkazana);

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
                var krajDana = datum.Value.Date.AddDays(1);
                sve = sve.Where(r => r.PocetakVreme < krajDana && r.KrajVreme > pocetakDana);
            }

            var rezervacije = sve.OrderBy(r => r.PocetakVreme).ToList();
            notifikacija("Dohvacene rezervacije po lokaciji i danu");
            return rezervacije;
        }
    }
}