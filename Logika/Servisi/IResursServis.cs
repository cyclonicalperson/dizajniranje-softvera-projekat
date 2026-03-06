using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
	public interface IResursServis
	{
		List<Resurs> dajResursePoLokacijiSortiranoPoTipu(int lokacijaId);
	}
}
