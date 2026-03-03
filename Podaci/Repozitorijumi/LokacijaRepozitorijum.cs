using Microsoft.EntityFrameworkCore;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Podaci.Repozitorijumi
{
    // Repozitorijum za upravljanje co-working lokacijama
    // Uključuje CRUD i izracunavanje statistika zauzetosti
    public class LokacijaRepozitorijum
    {
        private readonly KontekstBaze kontekst;

        public LokacijaRepozitorijum(KontekstBaze kontekst)
        {
            this.kontekst = kontekst;
        }

        // Vraca sve lokacije sa listom resursa
        public List<Lokacija> DajSve()
        {
            return kontekst.Lokacije
                .Include(l => l.Resursi)
                .ToList();
        }

        // Vraca lokaciju po ID-u sa resursima, ili null
        public Lokacija? DajPoId(int id)
        {
            return kontekst.Lokacije
                .Include(l => l.Resursi)
                .FirstOrDefault(l => l.Id == id);
        }

        // Dodaje novu lokaciju
        public void Dodaj(Lokacija lokacija)
        {
            kontekst.Lokacije.Add(lokacija);
            kontekst.SaveChanges();
        }

        // Azurira lokaciju
        public void Azuriraj(Lokacija lokacija)
        {
            kontekst.Lokacije.Update(lokacija);
            kontekst.SaveChanges();
        }

        // Brise lokaciju (kaskadno briše resurse na njoj)
        public void Obrisi(int id)
        {
            var lokacija = kontekst.Lokacije.Find(id);
            if (lokacija != null)
            {
                kontekst.Lokacije.Remove(lokacija);
                kontekst.SaveChanges();
            }
        }

        // Izracunava statistike zauzetosti lokacije za dati trenutak
        // Vraca ukupan broj resursa, broj zauzeto i procenat
        public StatistikaZauzetosti DajStatistikuZauzetosti(int lokacijaId, DateTime uTrenutku)
        {
            int ukupno = kontekst.Resursi.Count(r => r.LokacijaId == lokacijaId);

            int zauzeto = kontekst.Rezervacije
                .Include(r => r.Resurs)
                .Count(r => r.Resurs.LokacijaId == lokacijaId
                         && r.StatusRezervacije == StatusRezervacije.Aktivna
                         && r.PocetakVreme <= uTrenutku
                         && r.KrajVreme > uTrenutku);

            double procenat = ukupno > 0
                ? Math.Round((double)zauzeto / ukupno * 100, 1)
                : 0;

            return new StatistikaZauzetosti
            {
                LokacijaId = lokacijaId,
                UkupnoResursa = ukupno,
                ZauzetihResursa = zauzeto,
                ProcenatZauzetosti = procenat
            };
        }
    }

    // Embedded klasa sa statistikama zauzetosti lokacije
    public class StatistikaZauzetosti
    {
        public int LokacijaId { get; set; }
        public int UkupnoResursa { get; set; }
        public int ZauzetihResursa { get; set; }
        public double ProcenatZauzetosti { get; set; }

        public override string ToString() =>
            $"{ZauzetihResursa}/{UkupnoResursa} zauzeto ({ProcenatZauzetosti}%)";
    }
}