using System;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
	public interface IKorisnikServis
	{
		Korisnik? getKorisnik(int id);
		bool dodajKorisnika(string ime, string prezime, string email, string? telefon,
			int tipClanstvaId, string datumPocetkaClanstva, string datumKrajaClanstva,
			string statusNaloga);
		bool obrisiKorisnika(string ime, string prezime);
		bool izmeniKorisnika(string ime, string prezime, string? noviEmail, string? noviTelefon,
			int? noviTipClanstvaId, string? noviDatumPocetkaClanstva,
			string? noviDatumKrajaClanstva, string? noviStatusNaloga);
		List<Korisnik> dajKorisnike(string? lokacija, string? tipClanstva, string? statusNaloga);
	}
}
