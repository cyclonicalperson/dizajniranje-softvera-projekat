using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
	public interface ITipClanstvaServis
	{
		bool dodajTipClanstva(string ime, decimal cena, int trajanje, int maxSatiPoMesecu, bool pristupSali, int? brojSatiUSaliMesecno);
	}
}
