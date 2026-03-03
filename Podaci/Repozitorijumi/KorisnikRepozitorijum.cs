using Microsoft.EntityFrameworkCore;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Podaci.Repozitorijumi
{
    // Repozitorijum za upravljanje korisnicima
    // Pruza CRUD operacije, pretragu, filtriranje i upite za sate rezervacija
    public class KorisnikRepozitorijum
    {
        private readonly KontekstBaze kontekst;

        public KorisnikRepozitorijum(KontekstBaze kontekst)
        {
            this.kontekst = kontekst;
        }

        // Vraca sve korisnike sa tipom clanstva
        public List<Korisnik> DajSve()
        {
            return kontekst.Korisnici
                .Include(k => k.TipClanstva)
                .ToList();
        }

        // Vraca korisnika po ID-u sa tipom clanstva i svim rezervacijama
        public Korisnik? DajPoId(int id)
        {
            return kontekst.Korisnici
                .Include(k => k.TipClanstva)
                .Include(k => k.Rezervacije)
                    .ThenInclude(r => r.Resurs)
                        .ThenInclude(res => res.Lokacija)
                .FirstOrDefault(k => k.Id == id);
        }

        // Vraca korisnika po email adresi, ili null ako ne postoji
        public Korisnik? DajPoEmailu(string email)
        {
            return kontekst.Korisnici
                .Include(k => k.TipClanstva)
                .FirstOrDefault(k => k.Email == email);
        }

        // Vraća filtrirani spisak korisnika
        // Svi parametri su opcioni — ignorisu se ako nisu prosledjeni
        public List<Korisnik> DajPoFiltru(
            int? lokacijaId = null,
            int? tipClanstvaId = null,
            StatusNaloga? status = null)
        {
            var upit = kontekst.Korisnici
                .Include(k => k.TipClanstva)
                .AsQueryable();

            if (tipClanstvaId.HasValue)
                upit = upit.Where(k => k.TipClanstvaId == tipClanstvaId.Value);

            if (status.HasValue)
                upit = upit.Where(k => k.StatusNaloga == status.Value);

            // Korisnici koji su imali rezervacije na toj lokaciji
            if (lokacijaId.HasValue)
            {
                var idNaLokaciji = kontekst.Rezervacije
                    .Include(r => r.Resurs)
                    .Where(r => r.Resurs.LokacijaId == lokacijaId.Value)
                    .Select(r => r.KorisnikId)
                    .Distinct();

                upit = upit.Where(k => idNaLokaciji.Contains(k.Id));
            }

            return upit.ToList();
        }

        // Dodaje novog korisnika i cuva u bazi
        public void Dodaj(Korisnik korisnik)
        {
            kontekst.Korisnici.Add(korisnik);
            kontekst.SaveChanges();
        }

        // Azurira postojećeg korisnika i cuva promene
        public void Azuriraj(Korisnik korisnik)
        {
            kontekst.Korisnici.Update(korisnik);
            kontekst.SaveChanges();
        }

        // Brise korisnika po ID-u
        public void Obrisi(int id)
        {
            var korisnik = kontekst.Korisnici.Find(id);
            if (korisnik != null)
            {
                kontekst.Korisnici.Remove(korisnik);
                kontekst.SaveChanges();
            }
        }

        // Proverava da li vec postoji korisnik sa datim emailom
        // excludeId se koristi pri izmeni da se ne blokira sam korisnik
        public bool EmailPostoji(string email, int? excludeId = null)
        {
            var upit = kontekst.Korisnici.Where(k => k.Email == email);
            if (excludeId.HasValue)
                upit = upit.Where(k => k.Id != excludeId.Value);
            return upit.Any();
        }

        // Vraca ukupan broj sati svih rezervacija korisnika u datom mesecu
        // Ne uracunava otkazane rezervacije
        public double DajUkupnoSatiMesecno(int korisnikId, int godina, int mesec)
        {
            return kontekst.Rezervacije
                .Where(r => r.KorisnikId == korisnikId
                         && r.PocetakVreme.Year == godina
                         && r.PocetakVreme.Month == mesec
                         && r.StatusRezervacije != StatusRezervacije.Otkazana)
                .ToList()
                .Sum(r => (r.KrajVreme - r.PocetakVreme).TotalHours);
        }

        // Vraca broj sati koriscenja sala za sastanke za korisnika u datom mesecu
        // Koristi se za proveru ogranicenja tipa clanstva (BrojSatiUSaliMesecno)
        public double DajSatiSalaMesecno(int korisnikId, int godina, int mesec)
        {
            // Uzimamo ID-jeve svih sala ('sala' tip resursa)
            var idSala = kontekst.Resursi
                .Where(r => r.TipResursa == TipResursa.Sala)
                .Select(r => r.Id)
                .ToList();

            return kontekst.Rezervacije
                .Where(r => r.KorisnikId == korisnikId
                         && r.PocetakVreme.Year == godina
                         && r.PocetakVreme.Month == mesec
                         && r.StatusRezervacije != StatusRezervacije.Otkazana
                         && idSala.Contains(r.ResursId))
                .ToList()
                .Sum(r => (r.KrajVreme - r.PocetakVreme).TotalHours);
        }
    }
}