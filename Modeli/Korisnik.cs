namespace CoWorkingManager.Modeli
{
    /// Predstavlja korisnika (člana) co-working prostora
    public class Korisnik
    {
        public int Id { get; set; }

        // Ime korisnika
        public string Ime { get; set; } = string.Empty;

        // Prezime korisnika
        public string Prezime { get; set; } = string.Empty;

        // Email adresa
        public string Email { get; set; } = string.Empty;

        // Broj telefona (opciono)
        public string? Telefon { get; set; }

        // Strani kljuc na tabelu TipoviClanstava
        public int TipClanstvaId { get; set; }

        // Navigaciono svojstvo ka tipu clanstva
        public TipClanstva TipClanstva { get; set; } = null!;

        // Datum početka važenja clanstva
        public DateTime DatumPocetkaClanstva { get; set; }

        // Datum isteka važenja clanstva
        public DateTime DatumKrajaClanstva { get; set; }

        // Status naloga, cuva se kao string u bazi:
        // 'aktivan' | 'pauziran' | 'istekao'
        public StatusNaloga StatusNaloga { get; set; } = StatusNaloga.Aktivan;

        // Rezervacije koje je ovaj korisnik napravio
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();

        // Pomocno svojstvo: puno ime za prikaz u GUI-u
        public string PunoIme => $"{Ime} {Prezime}";

        public override string ToString() => PunoIme;
    }
}