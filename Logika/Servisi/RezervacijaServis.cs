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
                if (!TimeOnly.TryParse(pocetakVreme, out TimeOnly t))
                {
                    notifikacija($"Neispravno vreme pocetka: '{pocetakVreme}'. Koristite format HH:mm (npr. 09:00)");
                    return false;
                }
                pocetak = pocetakDatum.Value.ToDateTime(t);
            }

            DateTime? kraj = null;
            if (krajDatum != null && krajVreme != null)
            {
                if (!TimeOnly.TryParse(krajVreme, out TimeOnly t))
                {
                    notifikacija($"Neispravno vreme zavrsetka: '{krajVreme}'. Koristite format HH:mm (npr. 17:00)");
                    return false;
                }
                kraj = krajDatum.Value.ToDateTime(t);
            }

            if (pocetak == null || kraj == null)
            {
                notifikacija("Datum i vreme pocetka i zavrsetka su obavezni");
                return false;
            }
            if (pocetak >= kraj)
            {
                notifikacija("Kraj rezervacije mora biti posle pocetka");
                return false;
            }
            if (pocetak.Value.Date != kraj.Value.Date)
            {
                notifikacija("Rezervacija mora poceti i zavrsiti se u istom danu");
                return false;
            }

            var lokacija = _fasada.Lokacije.DajPoId(resurs.LokacijaId);
            if (lokacija == null)
            {
                notifikacija("Kreiranje rezervacije neuspesno — lokacija resursa nije pronadjena");
                return false;
            }

            if (korisnik.TipClanstva == null)
            {
                notifikacija("Korisnik nema dodeljen tip clanstva");
                return false;
            }

            if (resurs.TipResursa == TipResursa.Sala && !korisnik.TipClanstva.PristupSali)
            {
                notifikacija("Korisnikov tip clanstva ne ukljucuje pristup salama za sastanke");
                return false;
            }

            if (!validacijaClanstva(korisnik, resurs, pocetak.Value, kraj.Value))
            {
                notifikacija("Kreiranje rezervacije neuspesno — prekoracen dozvoljeni broj sati u okviru clanstva");
                return false;
            }
            if (!validacijaTermina(resurs, pocetak.Value, kraj.Value))
            {
                notifikacija("Kreiranje rezervacije neuspesno — resurs je vec zauzet u tom terminu");
                return false;
            }
            if (!validacijaRadnogVremena(lokacija, pocetak.Value, kraj.Value))
            {
                notifikacija($"Kreiranje rezervacije neuspesno — termin je van radnog vremena lokacije ({lokacija.RadniSati})");
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
                .FirstOrDefault(r => r.Korisnik.Id == korisnik.Id && r.Resurs.Id == resurs.Id
                                  && r.StatusRezervacije == StatusRezervacije.Aktivna);
            if (rezervacija == null)
            {
                notifikacija("Nije pronadjena aktivna rezervacija za ovog korisnika i resurs");
                return false;
            }
            if (_fasada.Rezervacije.Otkazi(rezervacija.Id))
            {
                notifikacija("Otkazana rezervacija");
                return true;
            }
            notifikacija("Otkazivanje rezervacije neuspesno");
            return false;
        }

        public bool izmeniRezervaciju(Korisnik korisnik, Resurs resurs, DateOnly? pocetakDatum, string? pocetakVreme, DateOnly? krajDatum, string? krajVreme)
        {
            var rezervacija = _fasada.Rezervacije.DajSve()
                .FirstOrDefault(r => r.Korisnik.Id == korisnik.Id && r.Resurs.Id == resurs.Id
                                  && r.StatusRezervacije == StatusRezervacije.Aktivna);
            if (rezervacija == null)
            {
                notifikacija("Nije pronadjena aktivna rezervacija za ovog korisnika i resurs");
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
                if (!TimeOnly.TryParse(pocetakVreme, out TimeOnly t))
                {
                    notifikacija($"Neispravno vreme pocetka: '{pocetakVreme}'. Koristite format HH:mm (npr. 09:00)");
                    return false;
                }
                pocetak = pocetakDatum.Value.ToDateTime(t);
            }

            DateTime? kraj = null;
            if (krajDatum != null && krajVreme != null)
            {
                if (!TimeOnly.TryParse(krajVreme, out TimeOnly t))
                {
                    notifikacija($"Neispravno vreme zavrsetka: '{krajVreme}'. Koristite format HH:mm (npr. 17:00)");
                    return false;
                }
                kraj = krajDatum.Value.ToDateTime(t);
            }

            if (pocetak == null) pocetak = rezervacija.PocetakVreme;
            if (kraj == null) kraj = rezervacija.KrajVreme;

            if (pocetak >= kraj)
            {
                notifikacija("Kraj rezervacije mora biti posle pocetka");
                return false;
            }
            if (pocetak.Value.Date != kraj.Value.Date)
            {
                notifikacija("Rezervacija mora poceti i zavrsiti se u istom danu");
                return false;
            }

            if (korisnik.TipClanstva == null)
            {
                notifikacija("Korisnik nema dodeljen tip clanstva");
                return false;
            }

            if (resurs.TipResursa == TipResursa.Sala && !korisnik.TipClanstva.PristupSali)
            {
                notifikacija("Korisnikov tip clanstva ne ukljucuje pristup salama za sastanke");
                return false;
            }

            if (!validacijaClanstva(korisnik, resurs, pocetak.Value, kraj.Value))
            {
                notifikacija("Prekoracen dozvoljeni broj sati u okviru clanstva");
                return false;
            }
            if (!validacijaTermina(resurs, pocetak.Value, kraj.Value))
            {
                notifikacija("Resurs je vec zauzet u tom terminu");
                return false;
            }

            var lokacijaObj = _fasada.Lokacije.DajPoId(resurs.LokacijaId);
            if (lokacijaObj == null)
            {
                notifikacija("Lokacija nije pronadjena");
                return false;
            }
            if (!validacijaRadnogVremena(lokacijaObj, pocetak.Value, kraj.Value))
            {
                notifikacija($"Termin je van radnog vremena lokacije ({lokacijaObj.RadniSati})");
                return false;
            }

            rezervacija.PocetakVreme = pocetak.Value;
            rezervacija.KrajVreme = kraj.Value;

            if (_fasada.Rezervacije.Azuriraj(rezervacija))
            {
                notifikacija("Izmenjena rezervacija");
                return true;
            }
            notifikacija("Izmena rezervacije neuspesna");
            return false;
        }

        private bool validacijaClanstva(Korisnik korisnik, Resurs resurs, DateTime pocetak, DateTime kraj)
        {
            if (DateOnly.FromDateTime(pocetak) < korisnik.DatumPocetkaClanstva ||
                DateOnly.FromDateTime(kraj) > korisnik.DatumKrajaClanstva)
                return false;

            int vremeClan;
            if (resurs.TipResursa == TipResursa.Sala)
            {
                if (korisnik.TipClanstva.BrojSatiUSaliMesecno == null)
                    return false;
                vremeClan = korisnik.TipClanstva.BrojSatiUSaliMesecno.Value;
            }
            else
            {
                vremeClan = korisnik.TipClanstva.MaxSatiPoMesecu;
            }

            if (vremeClan != 0)
            {
                foreach (Rezervacija r in _fasada.Rezervacije.DajZaMesecniIzvestaj(pocetak.Year, pocetak.Month))
                {
                    if (r.Korisnik.Id != korisnik.Id) continue;
                    if (resurs.TipResursa == TipResursa.Sala && r.Resurs.TipResursa != TipResursa.Sala) continue;
                    if (resurs.TipResursa != TipResursa.Sala && r.Resurs.TipResursa == TipResursa.Sala) continue;
                    vremeClan -= (int)r.TrajanjeSati;
                }
            }

            int trajanje = (int)(kraj - pocetak).TotalHours;
            return (vremeClan - trajanje) >= 0;
        }

        private bool validacijaTermina(Resurs resurs, DateTime pocetak, DateTime kraj)
        {
            return !_fasada.Rezervacije.PostojiPreklapanje(resurs.Id, pocetak, kraj);
        }

        private bool validacijaRadnogVremena(Lokacija lokacija, DateTime pocetakRez, DateTime krajRez)
        {
            var separatori = new[] { "–", "-" };
            string[] parts = lokacija.RadniSati.Split(separatori, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2) return false;

            if (!TimeSpan.TryParse(parts[0].Trim(), out TimeSpan pocetakRadnog)) return false;
            if (!TimeSpan.TryParse(parts[1].Trim(), out TimeSpan krajRadnog)) return false;

            TimeSpan pocetak = pocetakRez.TimeOfDay;
            TimeSpan kraj = krajRez.TimeOfDay;
            return pocetak >= pocetakRadnog && kraj <= krajRadnog;
        }

        public List<Rezervacija> dajRezervacijeKorisnika(string? imePrezime)
        {
            if (imePrezime == null)
            {
                return _fasada.Rezervacije.DajSve().OrderByDescending(r => r.PocetakVreme).ToList();
            }

            string[] delovi = imePrezime.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (delovi.Length < 2)
            {
                notifikacija("Unesite ime i prezime korisnika (npr. Marko Jovanovic)");
                return new List<Rezervacija>();
            }

            var ime = delovi[0];
            var prezime = delovi[1];

            var korisnik = _fasada.Korisnici.DajSve()
                .FirstOrDefault(k => k.Ime == ime && k.Prezime == prezime);
            if (korisnik == null)
            {
                notifikacija("Korisnik nije pronadjen");
                return new List<Rezervacija>();
            }

            var rezervacije = _fasada.Rezervacije.DajPoKorisniku(korisnik.Id);
            notifikacija("Dohvacene rezervacije korisnika");
            return rezervacije;
        }

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