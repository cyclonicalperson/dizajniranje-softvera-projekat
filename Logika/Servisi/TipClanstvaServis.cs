using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Logika.Servisi
{
	public class TipClanstvaServis : BazniServis, ITipClanstvaServis
	{
		private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();

		public bool dodajTipClanstva(TipClanstva tipClanstva)
		{
			if(_fasada.TipoviClanstva.Dodaj(tipClanstva))
			{
                notifikacija("Dodat novi tip clanstva");
				return true;
            }
			notifikacija("Dodavanje tipa clanstva neuspesno");
			return false;
		}
	}
}
