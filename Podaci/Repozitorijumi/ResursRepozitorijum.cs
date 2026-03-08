using Microsoft.EntityFrameworkCore;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Podaci.Repozitorijumi
{
    // Repozitorijum za upravljanje resursima (stola, sale, kancelarije)
    // Pruza filtriranje po tipu/lokaciji i provere dostupnosti u terminu
    //
    // !!!!!!!!!!!!
    // Za kreiranje novih resursa koristiti ResursFactory umesto direktnog new Resurs():
    //   RepozitorijumResursa.Dodaj(ResursFactory.KreirajSto(...))
    //   RepozitorijumResursa.Dodaj(ResursFactory.KreirajSalu(...))
    //   RepozitorijumResursa.Dodaj(ResursFactory.KreirajPrivatnuKancelariju(...))
    // !!!!!!!!!!!!
    public class ResursRepozitorijum
    {
        private readonly KontekstBaze _kontekst;

        public ResursRepozitorijum(KontekstBaze kontekst)
        {
            _kontekst = kontekst;
        }

        // Vraca sve resurse sa lokacijom
        public List<Resurs> DajSve()
        {
            return _kontekst.Resursi
                .Include(r => r.Lokacija)
                .ToList();
        }

        // Vraca resurs po ID-u sa lokacijom i rezervacijama, ili null
        public Resurs? DajPoId(int id)
        {
            return _kontekst.Resursi
                .Include(r => r.Lokacija)
                .Include(r => r.Rezervacije)
                .FirstOrDefault(r => r.Id == id);
        }

        // Vraca sve resurse na datoj lokaciji
        public List<Resurs> DajPoLokaciji(int lokacijaId)
        {
            return _kontekst.Resursi
                .Include(r => r.Lokacija)
                .Where(r => r.LokacijaId == lokacijaId)
                .ToList();
        }

        // Vraca sva radna mesta ('sto') na datoj lokaciji
        public List<Resurs> DajStoloviPoLokaciji(int lokacijaId)
        {
            return _kontekst.Resursi
                .Where(r => r.LokacijaId == lokacijaId && r.TipResursa == TipResursa.Sto)
                .ToList();
        }

        // Vraca sve sale za sastanke ('sala') na datoj lokaciji
        public List<Resurs> DajSalePoLokaciji(int lokacijaId)
        {
            return _kontekst.Resursi
                .Where(r => r.LokacijaId == lokacijaId && r.TipResursa == TipResursa.Sala)
                .ToList();
        }

        // Vraca sve privatne kancelarije na datoj lokaciji
        public List<Resurs> DajKancelarijePoLokaciji(int lokacijaId)
        {
            return _kontekst.Resursi
                .Where(r => r.LokacijaId == lokacijaId && r.TipResursa == TipResursa.PrivatnaKancelarija)
                .ToList();
        }

        // Proverava da li je resurs slobodan u traženom terminu
        // excludeRezervacijaId se koristi pri izmeni da se ne blokira stara rezervacija
        public bool JeDostupan(int resursId, DateTime pocetak, DateTime kraj, int? excludeRezervacijaId = null)
        {
            var upit = _kontekst.Rezervacije
                .Where(r => r.ResursId == resursId
                         && r.StatusRezervacije != StatusRezervacije.Otkazana
                         && r.PocetakVreme < kraj
                         && r.KrajVreme > pocetak);

            if (excludeRezervacijaId.HasValue)
                upit = upit.Where(r => r.Id != excludeRezervacijaId.Value);

            return !upit.Any();
        }

        // Vraca sve slobodne resurse na lokaciji u traženom terminu
        public List<Resurs> DajDostupneResurse(int lokacijaId, DateTime pocetak, DateTime kraj)
        {
            return DajPoLokaciji(lokacijaId)
                .Where(r => JeDostupan(r.Id, pocetak, kraj))
                .ToList();
        }

        // Dodaje novi resurs
        // Vraca false ako na istoj lokaciji vec postoji resurs sa istim imenom
        // Preporuka: koristiti ResursFactory za kreiranje resursa
        public bool Dodaj(Resurs resurs)
        {
            if (_kontekst.Resursi.Any(r => r.LokacijaId == resurs.LokacijaId && r.Ime == resurs.Ime))
                return false;

            _kontekst.Resursi.Add(resurs);
            _kontekst.SaveChanges();
            return true;
        }

        // Azurira resurse
        // Vraca false ako resurs sa datim ID-jem ne postoji
        public bool Azuriraj(Resurs resurs)
        {
            if (!_kontekst.Resursi.AsNoTracking().Any(r => r.Id == resurs.Id))
                return false;

            var tracked = _kontekst.ChangeTracker.Entries<Resurs>()
                .FirstOrDefault(e => e.Entity.Id == resurs.Id);
            if (tracked != null)
                tracked.State = EntityState.Detached;

            _kontekst.Entry(resurs).State = EntityState.Modified;
            _kontekst.SaveChanges();
            return true;
        }

        // Brise resurs po ID-u
        // Vraca false ako resurs ne postoji
        public bool Obrisi(int id)
        {
            var resurs = _kontekst.Resursi.Find(id);
            if (resurs == null)
                return false;

            _kontekst.Resursi.Remove(resurs);
            _kontekst.SaveChanges();
            return true;
        }
    }
}