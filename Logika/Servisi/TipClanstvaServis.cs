using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.Logika.Servisi
{
	public class TipClanstvaServis : BazniServis, ITipClanstvaServis
	{
		private readonly CoworkingFasada _fasada = CoworkingFasada.DajInstancu();

		public bool dodajTipClanstva(string ime, decimal cena, int trajanje, int maxSatiPoMesecu, bool pristupSali, int? brojSatiUSaliMesecno)
		{
			var tipClanstva = new TipClanstva
			{
				Ime = ime,
				Cena = cena,
				Trajanje = trajanje,
				MaxSatiPoMesecu = maxSatiPoMesecu,
				PristupSali = pristupSali,
				BrojSatiUSaliMesecno = brojSatiUSaliMesecno
			};
            if (_fasada.TipoviClanstva.Dodaj(tipClanstva))
			{
                notifikacija("Dodat novi tip clanstva");
				return true;
            }
			notifikacija("Dodavanje tipa clanstva neuspesno");
			return false;
		}
	}
}
