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

        // Dodaje novog administratora
        // Vraca false ako korisnicko ime ili email već postoje
        public bool Dodaj(Administrator administrator)
        {
            if (KorisnickoImePostoji(administrator.KorisnickoIme))
                return false;

            if (EmailPostoji(administrator.Email))
                return false;

            kontekst.Administratori.Add(administrator);
            kontekst.SaveChanges();
            return true;
        }

        // Azurira postojeceg administratora
        // Vraca false ako administrator ne postoji, ili ako novo korisnicko
        // ime / email već koristi drugi administrator
        public bool Azuriraj(Administrator administrator)
        {
            if (!kontekst.Administratori.Any(a => a.Id == administrator.Id))
                return false;

            if (KorisnickoImePostoji(administrator.KorisnickoIme, excludeId: administrator.Id))
                return false;

            if (EmailPostoji(administrator.Email, excludeId: administrator.Id))
                return false;

            kontekst.Administratori.Update(administrator);
            kontekst.SaveChanges();
            return true;
        }

        // Brise administratora po ID-u
        // Vraca false ako administrator ne postoji
        public bool Obrisi(int id)
        {
            var admin = kontekst.Administratori.Find(id);
            if (admin == null)
                return false;

            kontekst.Administratori.Remove(admin);
            kontekst.SaveChanges();
            return true;
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