using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using System.Windows.Markup;

namespace CoWorkingManager.Logika.Servisi
{
	public class ResursServis : BazniServis, IResursServis
	{
		private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();

        public Resurs getResurs(int id)
        {
            var resurs = _fasada.Resursi.DajPoId(id);
            if (resurs == null) notifikacija("Resurs je null");
            else notifikacija("Uzet resurs");
            return resurs;
        }

        public bool dodajResurs(string ime, string imeLokacije, string tipResursa, 
            string? opis, string? podTipStola, int? kapacitet, bool? imaProjektor, bool? imaTV, bool? imaTablu, bool? imaOnlineOpremu)
        {
            Lokacija lokacija = _fasada.Lokacije.DajPoNazivu(imeLokacije);
            if (lokacija == null)
            {
                notifikacija("Dodavanje resursa neuspesno jer lokacija nije pronadjena");
                return false;
            }
            TipResursa tip = Enum.Parse<TipResursa>(tipResursa);
            PodtipStola? podTip = null;
            if (podTipStola != null && tip != TipResursa.Sto)
            {
                notifikacija("Podtip stola moze biti postavljen samo za resurse tipa Sto");
                return false;
            }
            else if (podTipStola != null && tip == TipResursa.Sto)
            {
                podTip = podTipStola != null ? Enum.Parse<PodtipStola>(podTipStola) : null;
            }
            var resurs = new Resurs
            {
                Ime = ime,
                LokacijaId = lokacija.Id,
                Lokacija = lokacija,
                TipResursa = tip,
                Opis = opis,
                PodtipStola = podTip,
                Kapacitet = kapacitet,
                ImaProjektor = imaProjektor,
                ImaTV = imaTV,
                ImaTablu = imaTablu,
                ImaOnlineOpremu = imaOnlineOpremu
            };
            if (_fasada.Resursi.Dodaj(resurs))
            {
                notifikacija("Novi resurs je dodat");
                return true;
            }
            notifikacija("Dodavanje novog resursa neuspesno");
            return false;
        }

        public bool obrisiResurs(string ime)
        {
            var resurs = _fasada.Resursi.DajSve()
                .FirstOrDefault(r => r.Ime == ime);
            if (resurs == null) 
            { 
                notifikacija("Brisanje resursa neuspesno jer resurs nije pronadjen"); 
                return false;
            }
            if (_fasada.Resursi.Obrisi(resurs.Id))
            {
                notifikacija("Obrisan resurs");
                return true;
            }
            notifikacija("Brisanje resursa neuspesno");
            return false;
        }

        public bool izmeniResurs(string ime, string? imeLokacije, string? tipResursa, 
            string? opis, string? podTipStola, int? kapacitet, bool? imaProjektor, bool? imaTV, bool? imaTablu, bool? imaOnlineOpremu)
        {
            var resurs = _fasada.Resursi.DajSve()
                .FirstOrDefault(r => r.Ime == ime);
            if (resurs == null) 
            { 
                notifikacija("Izmena resursa neuspesna jer resurs nije pronadjen"); 
                return false; 
            }
            if(!string.IsNullOrWhiteSpace(imeLokacije))
            {
                var lokacija = _fasada.Lokacije.DajPoNazivu(imeLokacije);
                if (lokacija == null)
                {
                    notifikacija("Izmena resursa neuspesna jer lokacija nije pronadjena");
                    return false;
                }
                resurs.LokacijaId = lokacija.Id;
                resurs.Lokacija = lokacija;
            }
            if (!string.IsNullOrWhiteSpace(tipResursa)) 
            {
                TipResursa tip = Enum.Parse<TipResursa>(tipResursa);
                resurs.TipResursa = tip; 
            }
            if(!string.IsNullOrWhiteSpace(opis)) resurs.Opis = opis;
            if(!string.IsNullOrWhiteSpace(podTipStola))
            {
                if (resurs.TipResursa != TipResursa.Sto)
                {
                    notifikacija("Podtip stola moze biti postavljen samo za resurse tipa Sto");
                    return false;
                }
                resurs.PodtipStola = Enum.Parse<PodtipStola>(podTipStola);
            }
            if(kapacitet != null) resurs.Kapacitet = kapacitet;
            if (imaProjektor != null) resurs.ImaProjektor = imaProjektor;
            if (imaTV != null) resurs.ImaTV = imaTV;
            if (imaTablu != null) resurs.ImaTablu = imaTablu;
            if (imaOnlineOpremu != null) resurs.ImaOnlineOpremu = imaOnlineOpremu;
            if (_fasada.Resursi.Azuriraj(resurs))
            {
                notifikacija("Izmenjen resurs");
                return true;
            }
            notifikacija("Izmene resursa neuspesne");
            return false;
        }
        public List<Resurs> dajResursePoLokacijiSortiranoPoTipu(int lokacijaId)
		{
			var resursi = _fasada.Resursi
				.DajPoLokaciji(lokacijaId)
				.OrderBy(r => r.TipResursa)
				.ToList();
			notifikacija("Dohvaceni resursi po lokaciji");
			return resursi;
		}


	}
}
