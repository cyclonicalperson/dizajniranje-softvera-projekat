using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public interface IResursServis
    {
        List<Resurs> dajSve();
        List<Resurs> dajResursePoLokacijiSortiranoPoTipu(string? lokacija);
    }
}
