using System;
using CoWorkingManager.Modeli;
namespace CoWorkingManager.Logika.Servisi
{
	public interface IKorisnikServis
	{
		Korisnik getKorisnik(int id);
		bool dodajKorisnika(Korisnik korisnik);
		bool obrisiKorisnika(int id);
		bool izmeniKorisnika(Korisnik korisnik);
	}
}
