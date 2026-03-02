namespace CoWorkingManager.Modeli
{
    // Bazna klasa za sve resurse u co-working prostoru
    // Koristi TPH (Table Per Hierarchy) strategiju mapiranja —
    // svi podtipovi se čuvaju u jednoj tabeli uz diskriminator kolonu TipResursa
    public class Resurs
    {
        public int Id { get; set; }

        // Strani ključ na lokaciju kojoj resurs pripada
        public int LokacijaId { get; set; }

        // Navigaciono svojstvo ka lokaciji
        public Lokacija Lokacija { get; set; } = null!;

        // Naziv ili oznaka resursa (npr. "Sto A4", "Sala Plava")
        public string Naziv { get; set; } = string.Empty;

        // Tip resursa — koristi se kao EF diskriminator
        public TipResursa TipResursa { get; set; }

        // Opcioni opis resursa
        public string? Opis { get; set; }

        // Rezervacije vezane za ovaj resurs
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();

        public override string ToString() => Naziv;
    }

    /// Radno mesto (hot desk ili dedicated desk)
    public class RadnoMesto : Resurs
    {
        // Pod-tip radnog mesta
        public PodtipRadnogMesta Podtip { get; set; }

        public RadnoMesto()
        {
            TipResursa = TipResursa.RadnoMesto;
        }
    }

    // Sala za sastanke sa opremom
    public class SalaZaSastanke : Resurs
    {
        // Broj mesta u sali
        public int Kapacitet { get; set; }

        // Da li sala ima projektor
        public bool ImaProjektor { get; set; }

        // Da li sala ima TV ekran
        public bool ImaTV { get; set; }

        // Da li sala ima tablu
        public bool ImaTablu { get; set; }

        // Da li sala ima opremu za online sastanke (kamera, mikrofon, zvučnici)
        public bool ImaOpреmuZaOnlajn { get; set; }

        public SalaZaSastanke()
        {
            TipResursa = TipResursa.SalaZaSastanke;
        }
    }

    /// Privatna kancelarija za iznajmljivanje
    public class PrivatnaKancelarija : Resurs
    {
        // Maksimalan broj osoba u kancelariji
        public int Kapacitet { get; set; }

        // Da li kancelarija ima zaključivi ormar/sef
        public bool ImaZakljuciveSkladiste { get; set; }

        // Da li kancelarija ima namensku telefonsku liniju
        public bool ImaNamenskiTelefon { get; set; }

        public PrivatnaKancelarija()
        {
            TipResursa = TipResursa.PrivatnaKancelarija;
        }
    }
}