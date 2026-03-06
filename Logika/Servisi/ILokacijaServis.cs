using System;
using CoWorkingManager.Modeli;
namespace CoWorkingManager.Logika.Servisi
{
    public interface ILokacijaServis
    {
        Lokacija getLokacija(int id);
        bool dodajLokaciju(Lokacija lokacija);
        bool obrisiLokaciju(int id);
        bool izmeniLokaciju(Lokacija lokacija);
    }
}
