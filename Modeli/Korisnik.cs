namespace CoWorkingManager.Modeli
{
    // Predstavlja korisnika (člana) co-working prostora.
    public class Korisnik
    {
        public int Id { get; set; }

        // Ime korisnika
        public string Ime { get; set; } = string.Empty;

        // Prezime korisnika
        public string Prezime { get; set; } = string.Empty;

        // Email adresa — mora biti jedinstven u bazi
        public string Email { get; set; } = string.Empty;

        // Broj telefona (opciono)
        public string? Telefon { get; set; }

        // Strani ključ na tip članstva
        public int TipClanstvaId { get; set; }

        // Navigaciono svojstvo ka tipu članstva
        public TipClanstva TipClanstva { get; set; } = null!;

        // Datum početka važenja članstva
        public DateTime DatumPocetka { get; set; }

        // Datum isteka važenja članstva
        public DateTime DatumIsteka { get; set; }

        // Trenutni status naloga
        public StatusNaloga Status { get; set; } = StatusNaloga.Aktivan;

        // Rezervacije koje je ovaj korisnik napravio
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();

        // Pomoćno svojstvo: puno ime
        public string PunoIme => $"{Ime} {Prezime}";

        public override string ToString() => PunoIme;
    }
}