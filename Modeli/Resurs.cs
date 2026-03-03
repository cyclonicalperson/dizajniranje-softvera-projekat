namespace CoWorkingManager.Modeli
{
    // Predstavlja resurs u co-working prostoru (sto, sala, privatna kancelarija).
    // Sve vrste resursa čuvaju se u jednoj tabeli Resursi uz diskriminator TipResursa
    // Kolone specifične za podtip su NULL za ostale tipove
    public class Resurs
    {
        public int Id { get; set; }

        // Strani kljuc na lokaciju kojoj resurs pripada
        public int LokacijaId { get; set; }

        // Navigaciono svojstvo ka lokaciji
        public Lokacija Lokacija { get; set; } = null!;

        // Ime ili oznaka resursa (npr. "Sto A-1", "Sala K-1")
        public string Ime { get; set; } = string.Empty;

        // Tip resursa, cuva se kao string u bazi:
        // 'sto' | 'sala' | 'privatna_kancelarija'
        public TipResursa TipResursa { get; set; }

        // Opcioni opis resursa
        public string? Opis { get; set; }

        // Rezervacije vezane za ovaj resurs
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();

        public override string ToString() => Naziv;

        // ────── Kolone specificne za radna mesta (TipResursa = 'sto') ──────────

        // Pod-tip radnog mesta, NULL za sale i kancelarije
        // 'hot_desk' | 'dedicated_desk'
        public PodtipStola? PodtipStola { get; set; }

        // ────── Kolone specificne za sale i privatne kancelarije ───────────────

        // Broj mesta, NULL za radna mesta
        public int? Kapacitet { get; set; }

        // Da li ima projektor, NULL za radna mesta
        public bool? ImaProjektor { get; set; }

        // Da li ima TV ekran, NULL za radna mesta
        public bool? ImaTV { get; set; }

        // Da li ima tablu, NULL za radna mesta
        public bool? ImaTablu { get; set; }

        // Da li ima opremu za online sastanke, NULL za radna mesta
        public bool? ImaOnlineOpremu { get; set; }

        // Rezervacije vezane za ovaj resurs
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();

        public override string ToString() => Ime;
    }
}