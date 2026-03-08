using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Logika.Servisi
{
    public class KorisnikServis : BazniServis, IKorisnikServis
    {
        private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();

        public Korisnik? getKorisnik(int id)
        {
            var korisnik = _fasada.Korisnici.DajPoId(id);
            if (korisnik == null) notifikacija("Korisnik je null");
            else notifikacija("Uzet korisnik");
            return korisnik;
        }

        public List<Korisnik> dajSve()
        {
            var korisnici = _fasada.Korisnici.DajSve();
            notifikacija("Dohvaceni svi korisnici");
            return korisnici;
        }

        public bool dodajKorisnika(string ime, string prezime, string email, string? telefon,
            string tipClanstva, DateOnly datumPocetkaClanstva, DateOnly datumKrajaClanstva,
            string statusNaloga)
        {
            if (string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime))
            {
                notifikacija("Ime i prezime su obavezna polja");
                return false;
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@') || email.IndexOf('.', email.IndexOf('@')) < 0)
            {
                notifikacija("Email adresa nije u ispravnom formatu (npr. korisnik@domen.com)");
                return false;
            }

            if (datumKrajaClanstva < datumPocetkaClanstva)
            {
                notifikacija("Datum kraja clanstva ne moze biti pre datuma pocetka");
                return false;
            }

            if (!Enum.TryParse<StatusNaloga>(statusNaloga, ignoreCase: true, out StatusNaloga status))
            {
                notifikacija($"Nevalidan status naloga: '{statusNaloga}'. Dozvoljene vrednosti: Aktivan, Pauziran, Istekao");
                return false;
            }

            TipClanstva? tip = _fasada.TipoviClanstva.DajPoImenu(tipClanstva);
            if (tip == null)
            {
                notifikacija("Ne postoji tip clanstva sa imenom " + tipClanstva);
                return false;
            }

            var korisnik = new Korisnik
            {
                Ime = ime,
                Prezime = prezime,
                Email = email,
                Telefon = telefon,
                TipClanstvaId = tip.Id,
                TipClanstva = tip,
                DatumPocetkaClanstva = datumPocetkaClanstva,
                DatumKrajaClanstva = datumKrajaClanstva,
                StatusNaloga = status
            };

            if (_fasada.Korisnici.Dodaj(korisnik))
            {
                notifikacija("Novi korisnik je dodat");
                return true;
            }
            notifikacija("Dodavanje novog korisnika neuspesno — korisnik sa tim emailom vec postoji");
            return false;
        }

        public bool obrisiKorisnika(string ime, string prezime)
        {
            // ISPRAVKA: Validacija praznih polja
            if (string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime))
            {
                notifikacija("Ime i prezime su obavezna polja");
                return false;
            }

            var korisnik = _fasada.Korisnici.DajSve()
                .FirstOrDefault(k => k.Ime == ime && k.Prezime == prezime);

            if (korisnik == null)
            {
                notifikacija("Brisanje korisnika neuspesno jer korisnik nije pronadjen");
                return false;
            }

            if (_fasada.Korisnici.Obrisi(korisnik.Id))
            {
                notifikacija("Obrisan korisnik");
                return true;
            }
            notifikacija("Brisanje korisnika neuspesno");
            return false;
        }

        public bool izmeniKorisnika(string ime, string prezime, string? noviEmail, string? noviTelefon,
            string? noviTipClanstva, DateOnly? noviDatumPocetkaClanstva,
            DateOnly? noviDatumKrajaClanstva, string? noviStatusNaloga)
        {
            if (string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime))
            {
                notifikacija("Ime i prezime su obavezna polja");
                return false;
            }

            var korisnik = _fasada.Korisnici.DajSve()
                .FirstOrDefault(k => k.Ime == ime && k.Prezime == prezime);

            if (korisnik == null)
            {
                notifikacija("Izmena korisnika neuspesna jer korisnik nije pronadjen");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(noviEmail))
            {
                if (!noviEmail.Contains('@') || noviEmail.IndexOf('.', noviEmail.IndexOf('@')) < 0)
                {
                    notifikacija("Nova email adresa nije u ispravnom formatu");
                    return false;
                }
                korisnik.Email = noviEmail;
            }

            if (!string.IsNullOrWhiteSpace(noviTelefon)) korisnik.Telefon = noviTelefon;

            if (!string.IsNullOrWhiteSpace(noviTipClanstva))
            {
                TipClanstva? tip = _fasada.TipoviClanstva.DajPoImenu(noviTipClanstva);
                if (tip == null)
                {
                    notifikacija("Ne postoji tip clanstva sa imenom " + noviTipClanstva);
                    return false;
                }
                korisnik.TipClanstva = tip;
                korisnik.TipClanstvaId = tip.Id;
            }

            DateOnly pocetakZaProveru = noviDatumPocetkaClanstva ?? korisnik.DatumPocetkaClanstva;
            DateOnly krajZaProveru = noviDatumKrajaClanstva ?? korisnik.DatumKrajaClanstva;
            if (krajZaProveru < pocetakZaProveru)
            {
                notifikacija("Datum kraja clanstva ne moze biti pre datuma pocetka");
                return false;
            }
            if (noviDatumPocetkaClanstva != null) korisnik.DatumPocetkaClanstva = noviDatumPocetkaClanstva.Value;
            if (noviDatumKrajaClanstva != null) korisnik.DatumKrajaClanstva = noviDatumKrajaClanstva.Value;

            if (!string.IsNullOrWhiteSpace(noviStatusNaloga))
            {
                // ISPRAVKA: Enum.TryParse umesto Enum.Parse
                if (!Enum.TryParse<StatusNaloga>(noviStatusNaloga, ignoreCase: true, out StatusNaloga noviStatus))
                {
                    notifikacija($"Nevalidan status naloga: '{noviStatusNaloga}'. Dozvoljene vrednosti: Aktivan, Pauziran, Istekao");
                    return false;
                }
                korisnik.StatusNaloga = noviStatus;
            }

            if (_fasada.Korisnici.Azuriraj(korisnik))
            {
                notifikacija("Izmenjen korisnik");
                return true;
            }
            notifikacija("Izmena korisnika neuspesna");
            return false;
        }

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

            StatusNaloga? status = null;
            if (!string.IsNullOrEmpty(statusNaloga) && Enum.TryParse(statusNaloga, out StatusNaloga parsed))
                status = parsed;

            var korisnici = _fasada.Korisnici.DajPoFiltru(lokacijaId, tipClanstvaId, status);
            notifikacija("Dohvacena lista korisnika");
            return korisnici;
        }

        public List<string> dajStatuseNaloga()
        {
            List<string> statusi = new List<string>();
            foreach (var korisnik in _fasada.Korisnici.DajSve())
            {
                if (!statusi.Contains(korisnik.StatusNaloga.ToString()))
                    statusi.Add(korisnik.StatusNaloga.ToString());
            }
            notifikacija("Dohvaceni statusi naloga");
            return statusi;
        }
    }
}