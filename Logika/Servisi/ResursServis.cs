using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Logika.Servisi
{
	public class ResursServis : BazniServis, IResursServis
	{
		private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();

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
