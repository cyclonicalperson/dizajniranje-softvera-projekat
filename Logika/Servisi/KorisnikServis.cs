using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using System.Windows.Media;

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

		// Prima sve podatke potrebne za kreiranje korisnika
		// Vraca true ako je korisnik uspesno dodat, false ako vec postoji korisnik sa istim emailom
		public bool dodajKorisnika(string ime, string prezime, string email, string? telefon,
			string tipClanstva, DateOnly datumPocetkaClanstva, DateOnly datumKrajaClanstva,
			string statusNaloga)
		{
			StatusNaloga status;
			status = Enum.Parse<StatusNaloga>(statusNaloga);
			TipClanstva tip = _fasada.TipoviClanstva.DajPoImenu(tipClanstva);
			if(tip == null)
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
			notifikacija("Dodavanje novog korisnika neuspesno");
			return false;
		}

        // Trazi korisnika po imenu i prezimenu i brise ga
        // Vraca true ako je korisnik uspesno obrisan, false ako korisnik nije pronadjen
        // Brise korisnika samo na osnovu ID-a koji se izvuce iz pronadjenog korisnika,
		// znaci null polja nemaju nikakvu ulogu u samom brisanju.
		// Jedino sto bi se promenilo je potpis funkcije, telo ostaje potpuno isto
        public bool obrisiKorisnika(string ime, string prezime)
		{
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

		// Trazi korisnika po imenu i prezimenu i menja mu samo ona polja koja nisu null
		// Vraca true ako je izmena uspesna, false ako korisnik nije pronadjen ili izmena neuspesna
		public bool izmeniKorisnika(string ime, string prezime, string? noviEmail, string? noviTelefon,
			string? noviTipClanstva, DateOnly? noviDatumPocetkaClanstva,
			DateOnly? noviDatumKrajaClanstva, string? noviStatusNaloga)
		{
			var korisnik = _fasada.Korisnici.DajSve()
				.FirstOrDefault(k => k.Ime == ime && k.Prezime == prezime);

			if (korisnik == null)
			{
				notifikacija("Izmena korisnika neuspesna jer korisnik nije pronadjen");
				return false;
			}
           
            if (noviEmail != null)                  korisnik.Email = noviEmail;
			if (noviTelefon != null)                korisnik.Telefon = noviTelefon;
			if (noviTipClanstva != null) 
			{
                TipClanstva? tip = _fasada.TipoviClanstva.DajPoImenu(noviTipClanstva) ?? korisnik.TipClanstva;
                korisnik.TipClanstva = tip; 
				korisnik.TipClanstvaId = tip.Id; 
			}
			if (noviDatumPocetkaClanstva != null)   korisnik.DatumPocetkaClanstva = (DateOnly)noviDatumPocetkaClanstva;
			if (noviDatumKrajaClanstva != null)     korisnik.DatumKrajaClanstva = (DateOnly)noviDatumKrajaClanstva;
			if (noviStatusNaloga != null) 
			{
                StatusNaloga? status;
                status = Enum.Parse<StatusNaloga>(noviStatusNaloga);
                korisnik.StatusNaloga = (StatusNaloga)status; 
			}

			if (_fasada.Korisnici.Azuriraj(korisnik))
			{
				notifikacija("Izmenjen korisnik");
				return true;
			}
			notifikacija("Izmena korisnika neuspesna");
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
