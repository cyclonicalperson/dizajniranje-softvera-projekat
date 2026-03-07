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

        public bool dodajResurs(string ime, string imeLokacije, string tipResursa, string? opis)
        {
            Lokacija lokacija = _fasada.Lokacije.DajSve()
                .FirstOrDefault(l => l.Ime == imeLokacije);
            TipResursa tip = Enum.Parse<TipResursa>(tipResursa);
            var resurs = new Resurs
            {
                Ime = ime,
                LokacijaId = lokacija.Id,
                Lokacija = lokacija,
                TipResursa = tip,
                Opis = opis
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
            if (_fasada.Resursi.Obrisi(resurs.Id))
            {
                notifikacija("Obrisan resurs");
                return true;
            }
            notifikacija("Brisanje resursa neuspesno");
            return false;
        }

        public bool izmeniResurs(string ime, string? imeLokacije, string? tipResursa, string? opis)
        {
            var resurs = _fasada.Resursi.DajSve()
                .FirstOrDefault(r => r.Ime == ime);
            if (resurs == null) 
            { 
                notifikacija("Izmena resursa neuspesna jer resurs nije pronadjen"); 
                return false; 
            }
            Lokacija lokacija = _fasada.Lokacije.DajSve()
                .FirstOrDefault(l => l.Ime == imeLokacije);
            TipResursa tip = tipResursa != null ? Enum.Parse<TipResursa>(tipResursa) : resurs.TipResursa;
            if (imeLokacije != null) 
            { 
                resurs.LokacijaId = lokacija.Id; 
                resurs.Lokacija = lokacija;
            }
            if(tipResursa != null) resurs.TipResursa = tip;
            if(opis != null) resurs.Opis = opis;
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
