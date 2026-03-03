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
        public void Dodaj(TipClanstva tip)
        {
            kontekst.TipoviClanstava.Add(tip);
            kontekst.SaveChanges();
        }

        // Azurira tip clanstva
        public void Azuriraj(TipClanstva tip)
        {
            kontekst.TipoviClanstava.Update(tip);
            kontekst.SaveChanges();
        }

        // Brise tip članstva samo ako nema korisnika sa tim paketom
        public void Obrisi(int id)
        {
            var tip = kontekst.TipoviClanstava.Find(id);
            if (tip == null) return;

            bool imaKorisnike = kontekst.Korisnici.Any(k => k.TipClanstvaId == id);
            if (imaKorisnike)
                throw new InvalidOperationException(
                    "Nije moguce obrisati tip clanstva koji ima dodeljene korisnike");

            kontekst.TipoviClanstava.Remove(tip);
            kontekst.SaveChanges();
        }
    }
}