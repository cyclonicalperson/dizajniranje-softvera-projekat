using Microsoft.EntityFrameworkCore;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Podaci.Repozitorijumi
{
    // Repozitorijum za upravljanje tipovima clanstva
    public class TipClanstvaRepozitorijum
    {
        private readonly KontekstBaze kontekst;

        public TipClanstvaRepozitorijum(KontekstBaze kontekst)
        {
            this.kontekst = kontekst;
        }

        // Vraca sve tipove clanstva
        public List<TipClanstva> DajSve() => kontekst.TipoviClanstava.ToList();

        // Vraca tip clanstva po ID-u, ili null
        public TipClanstva? DajPoId(int id) => kontekst.TipoviClanstava.Find(id);

        // Vraca tip clanstva po imenu, ili null
        public TipClanstva? DajPoImenu(string ime) =>
            kontekst.TipoviClanstava.FirstOrDefault(t => t.Ime == ime);

        // Dodaje novi tip clanstva
        // Vraca false ako tip sa istim nazivom već postoji
        public bool Dodaj(TipClanstva tip)
        {
            if (kontekst.TipoviClanstava.Any(t => t.Ime == tip.Ime))
                return false;

            kontekst.TipoviClanstava.Add(tip);
            kontekst.SaveChanges();
            return true;
        }

        // Azurira tip clanstva
        // Vraca false ako tip sa datim ID-jem ne postoji
        public bool Azuriraj(TipClanstva tip)
        {
            if (!kontekst.TipoviClanstava.AsNoTracking().Any(t => t.Id == tip.Id))
                return false;

            var tracked = kontekst.ChangeTracker.Entries<TipClanstva>()
                .FirstOrDefault(e => e.Entity.Id == tip.Id);
            if (tracked != null)
                tracked.State = EntityState.Detached;

            kontekst.Entry(tip).State = EntityState.Modified;
            kontekst.SaveChanges();
            return true;
        }

        // Brise tip clanstva samo ako nema korisnika sa tim paketom
        // Vraca false ako tip ne postoji ili ima dodeljene korisnike
        public bool Obrisi(int id)
        {
            var tip = kontekst.TipoviClanstava.Find(id);
            if (tip == null)
                return false;

            bool imaKorisnike = kontekst.Korisnici.Any(k => k.TipClanstvaId == id);
            if (imaKorisnike)
                return false;

            kontekst.TipoviClanstava.Remove(tip);
            kontekst.SaveChanges();
            return true;
        }
    }
}