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
            var resurs = new Resurs
            {
                Ime = ime,
                LokacijaId = lokacija.Id,
                Lokacija = lokacija,
                TipResursa = tip,
                Opis = opis
            };
            if (tip != TipResursa.Sto)
            {
                if(podTipStola != null)
                {
                    notifikacija("Podtip stola moze biti postavljen samo za resurse tipa Sto");
                    return false;
                }
                if(kapacitet != null)
                    resurs.Kapacitet = kapacitet;
                if(imaProjektor != null)
                    resurs.ImaProjektor = imaProjektor;
                if(imaTV != null)
                    resurs.ImaTV = imaTV;
                if(imaTablu != null)
                    resurs.ImaTablu = imaTablu;
                if(imaOnlineOpremu != null)
                    resurs.ImaOnlineOpremu = imaOnlineOpremu;
            }
            else
            {
                if(kapacitet != null || imaProjektor != null || imaTV != null || imaTablu != null || imaOnlineOpremu != null)
                {
                    notifikacija("Kapacitet, imaProjektor, imaTV, imaTablu i imaOnlineOpremu mogu biti postavljeni samo za resurse koji nisu tipa Sto");
                    return false;
                }
                PodtipStola? podTip = null;
                if (!string.IsNullOrWhiteSpace(podTipStola))
                {
                    podTip = Enum.Parse<PodtipStola>(podTipStola);
                    if (!Enum.IsDefined(typeof(PodtipStola), podTipStola))
                    {
                        notifikacija("Nevalidan podtip stola");
                        return false;
                    }
                }
                resurs.PodtipStola = podTip;
            }
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
            if (!string.IsNullOrWhiteSpace(opis)) resurs.Opis = opis;
            if (resurs.TipResursa != TipResursa.Sto)
            {
                if (podTipStola != null)
                {
                    notifikacija("Podtip stola moze biti postavljen samo za resurse tipa Sto");
                    return false;
                }
                if (kapacitet != null) resurs.Kapacitet = kapacitet;
                if (imaProjektor != null) resurs.ImaProjektor = imaProjektor;
                if (imaTV != null) resurs.ImaTV = imaTV;
                if (imaTablu != null) resurs.ImaTablu = imaTablu;
                if (imaOnlineOpremu != null) resurs.ImaOnlineOpremu = imaOnlineOpremu;
            }
            else {                 
                if (kapacitet != null || imaProjektor != null || imaTV != null || imaTablu != null || imaOnlineOpremu != null)
                {
                    notifikacija("Kapacitet, imaProjektor, imaTV, imaTablu i imaOnlineOpremu mogu biti postavljeni samo za resurse koji nisu tipa Sto");
                    return false;
                }
                if (!string.IsNullOrWhiteSpace(podTipStola))
                {
                    if (resurs.TipResursa != TipResursa.Sto)
                    {
                        notifikacija("Podtip stola moze biti postavljen samo za resurse tipa Sto");
                        return false;
                    }
                    resurs.PodtipStola = Enum.Parse<PodtipStola>(podTipStola);
                }
            }
            if (_fasada.Resursi.Azuriraj(resurs))
            {
                notifikacija("Izmenjen resurs");
                return true;
            }
            notifikacija("Izmene resursa neuspesne");
            return false;
        }
        public List<Resurs> dajResursePoLokacijiSortiranoPoTipu(string? lokacija)
		{
            if (lokacija == null)
            {
                var sviResursi = _fasada.Resursi.DajSve()
                    .OrderBy(r => r.TipResursa)
                    .ToList();
                return sviResursi;
            }
            var lokacijaObj = _fasada.Lokacije.DajPoNazivu(lokacija);
            if (lokacijaObj == null)
            {
                notifikacija("Dohvacanje resursa po lokaciji neuspesno jer lokacija nije pronadjena");
                return new List<Resurs>();
            }
            var resursi = _fasada.Resursi
				.DajPoLokaciji(lokacijaObj.Id)
				.OrderBy(r => r.TipResursa)
				.ToList();
			notifikacija("Dohvaceni resursi po lokaciji");
			return resursi;
		}


	}
}
