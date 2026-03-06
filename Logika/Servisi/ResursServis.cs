using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

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

        public bool dodajResurs(Resurs resurs)
        {
            if (_fasada.Resursi.Dodaj(resurs))
            {
                notifikacija("Novi resurs je dodat");
                return true;
            }
            notifikacija("Dodavanje novog resursa neuspesno");
            return false;
        }

        public bool obrisiResurs(int id)
        {
            if (_fasada.Resursi.Obrisi(id))
            {
                notifikacija("Obrisan resurs");
                return true;
            }
            notifikacija("Brisanje resursa neuspesno");
            return false;
        }

        public bool izmeniResurs(Resurs resurs)
        {
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
