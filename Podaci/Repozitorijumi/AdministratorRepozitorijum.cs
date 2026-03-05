using CoWorkingManager.Modeli;

namespace CoWorkingManager.Podaci.Repozitorijumi
{
    // Repozitorijum za upravljanje administratorima
    // Koristi se od strane AuthService za proveru kredencijala pri prijavi
    public class AdministratorRepozitorijum
    {
        private readonly KontekstBaze kontekst;

        public AdministratorRepozitorijum(KontekstBaze kontekst)
        {
            this.kontekst = kontekst;
        }

        public List<Administrator> DajSve()
        {
            return kontekst.Administratori.ToList();
        }

        public Administrator? DajPoId(int id)
        {
            return kontekst.Administratori.Find(id);
        }

        // Koristi se pri prijavi — proverava korisničko ime
        public Administrator? DajPoKorisnickomImenu(string korisnickoIme)
        {
            return kontekst.Administratori
                .FirstOrDefault(a => a.KorisnickoIme == korisnickoIme);
        }

        public Administrator? DajPoEmailu(string email)
        {
            return kontekst.Administratori
                .FirstOrDefault(a => a.Email == email);
        }

        public void Dodaj(Administrator administrator)
        {
            kontekst.Administratori.Add(administrator);
            kontekst.SaveChanges();
        }

        public void Azuriraj(Administrator administrator)
        {
            kontekst.Administratori.Update(administrator);
            kontekst.SaveChanges();
        }

        public void Obrisi(int id)
        {
            var admin = kontekst.Administratori.Find(id);
            if (admin != null)
            {
                kontekst.Administratori.Remove(admin);
                kontekst.SaveChanges();
            }
        }

        public bool KorisnickoImePostoji(string korisnickoIme, int? excludeId = null)
        {
            var upit = kontekst.Administratori.Where(a => a.KorisnickoIme == korisnickoIme);
            if (excludeId.HasValue)
                upit = upit.Where(a => a.Id != excludeId.Value);
            return upit.Any();
        }

        public bool EmailPostoji(string email, int? excludeId = null)
        {
            var upit = kontekst.Administratori.Where(a => a.Email == email);
            if (excludeId.HasValue)
                upit = upit.Where(a => a.Id != excludeId.Value);
            return upit.Any();
        }
    }
}