namespace CoWorkingManager.Modeli
{
    // Status korisničkog naloga
    // Vrednosti se čuvaju malim slovima u bazi
    public enum StatusNaloga
    {
        // Nalog je aktivan - cuva se kao 'aktivan'
        Aktivan,

        // Nalog je privremeno pauziran - cuva se kao 'pauziran'
        Pauziran,

        // Clanstvo je isteklo - cuva se kao 'istekao'
        Istekao
    }

    // Tip resursa u prostoru
    // Vrednosti se cuvaju malim slovima u bazi
    public enum TipResursa
    {
        // Radno mesto (hot desk ili dedicated desk) - 'sto'
        Sto,

        // Sala za sastanke - 'sala'
        Sala,

        // Privatna kancelarija - 'privatna_kancelarija'
        PrivatnaKancelarija
    }

    // Pod-tip radnog mesta
    // Vrednosti se cuvaju malim slovima u bazi
    public enum PodtipStola
    {
        // Slobodan sto, rezervise se po potrebi - 'hot_desk'
        HotDesk,

        // Namenski sto, uvek za istog korisnika - 'dedicated_desk'
        DedicatedDesk
    }

    /// Status rezervacije
    /// Vrednosti se cuvaju malim slovima u bazi
    public enum StatusRezervacije
    {
        // Rezervacija je aktivna → 'aktivna'
        Aktivna,

        // Rezervacija je uspesno završena - 'zavrsena'
        Zavrsena,

        // Rezervacija je otkazana - 'otkazana'
        Otkazana
    }
}