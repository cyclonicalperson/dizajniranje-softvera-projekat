using System;
using CoWorkingManager.Modeli;
namespace CoWorkingManager.Logika.Servisi
{
    public interface ILokacijaServis
    {
        Lokacija getLokacija(int id);
        List<Lokacija> dajSve();
        bool dodajLokaciju(string ime, string adresa, string grad, string radniSati, int maxBrojKorisnika);
        bool obrisiLokaciju(string ime);
        bool izmeniLokaciju(string ime, string? adresa, string? grad, string? radniSati, int? maxBrojKorisnika);
    }
}
