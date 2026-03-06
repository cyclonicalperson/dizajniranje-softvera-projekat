namespace CoWorkingManager.Modeli
{
    // Predstavlja administratora aplikacije
    public class Administrator
    {
        public int Id { get; set; }

        // Korisničko ime za prijavu
        public string KorisnickoIme { get; set; } = string.Empty;

        // BCrypt hash lozinke (cost factor 11)
        // Nikad se ne poredi direktno — koristiti BCrypt.Net.BCrypt.Verify(plaintext, HashLozinke)
        public string HashLozinke { get; set; } = string.Empty;

        // Ime administratora
        public string Ime { get; set; } = string.Empty;
        
        // Prezime administratora
        public string Prezime { get; set; } = string.Empty;

        // Email
        public string Email { get; set; } = string.Empty;

        // Datum kada je nalog kreiran
        public DateOnly DatumKreiranja { get; set; }

        // Pomoćno svojstvo — nije u bazi
        public string PunoIme => $"{Ime} {Prezime}";

        public override string ToString() => $"{PunoIme} ({KorisnickoIme})";
    }
}