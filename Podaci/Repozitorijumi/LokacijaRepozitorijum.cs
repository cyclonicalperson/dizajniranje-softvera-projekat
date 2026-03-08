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

        // Vraca lokaciju ciji naziv tacno odgovara trazenom — ili null ako ne postoji
        public Lokacija? DajPoNazivu(string naziv)
        {
            return kontekst.Lokacije
                .Include(l => l.Resursi)
                .FirstOrDefault(l => l.Ime == naziv);
        }

        // Dodaje novu lokaciju
        // Vraca false ako lokacija sa istim imenom i gradom vec postoji
        public bool Dodaj(Lokacija lokacija)
        {
            if (kontekst.Lokacije.Any(l => l.Ime == lokacija.Ime && l.Grad == lokacija.Grad))
                return false;

            kontekst.Lokacije.Add(lokacija);
            kontekst.SaveChanges();
            return true;
        }

        // Azurira lokaciju
        // Vraca false ako lokacija ne postoji
        public bool Azuriraj(Lokacija lokacija)
        {
            if (!kontekst.Lokacije.Any(l => l.Id == lokacija.Id))
                return false;

            kontekst.Lokacije.Update(lokacija);
            kontekst.SaveChanges();
            return true;
        }

        // Brise lokaciju (kaskadno briše resurse na njoj)
        // Vraca false ako lokacija ne postoji
        public bool Obrisi(int id)
        {
            var lokacija = kontekst.Lokacije.Find(id);
            if (lokacija == null)
                return false;

            kontekst.Lokacije.Remove(lokacija);
            kontekst.SaveChanges();
            return true;
        }

        // Izracunava statistike zauzetosti lokacije za dati trenutak
        // Vraca ukupan broj resursa, broj zauzeto i procenat
        public StatistikaZauzetosti DajStatistikuZauzetosti(int lokacijaId, DateTime uTrenutku)
        {
            var lokacija = kontekst.Lokacije
                .Include(l => l.Resursi)
                .FirstOrDefault(l => l.Id == lokacijaId);

            if (lokacija == null)
                throw new InvalidOperationException($"Lokacija sa ID {lokacijaId} ne postoji");

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
                Lokacija = lokacija,
                UkupnoResursa = ukupno,
                ZauzetihResursa = zauzeto,
                ProcenatZauzetosti = procenat
            };
        }

        // Izracunava statistike zauzetosti za sve lokacije u datom trenutku
        // Vraca listu sa ukupnim brojem resursa, brojem zauzetih i procentom za svaku lokaciju
        public List<StatistikaZauzetosti> DajStatistikuZauzetostiZaSve(DateTime uTrenutku)
        {
            var lokacije = kontekst.Lokacije
                .Include(l => l.Resursi)
                .ToList();

            return lokacije.Select(lokacija =>
            {
                int ukupnoResursa = kontekst.Resursi
                    .Count(r => r.LokacijaId == lokacija.Id);

                int zauzetihResursa = kontekst.Rezervacije
                    .Include(r => r.Resurs)
                    .Count(r => r.Resurs.LokacijaId == lokacija.Id
                             && r.StatusRezervacije == StatusRezervacije.Aktivna
                             && r.PocetakVreme <= uTrenutku
                             && r.KrajVreme > uTrenutku);

                double procenat = ukupnoResursa > 0
                    ? Math.Round((double)zauzetihResursa / ukupnoResursa * 100, 1)
                    : 0;

                return new StatistikaZauzetosti
                {
                    Lokacija = lokacija,
                    UkupnoResursa = ukupnoResursa,
                    ZauzetihResursa = zauzetihResursa,
                    ProcenatZauzetosti = procenat
                };
            }).ToList();
        }
    }

    // Embedded klasa sa statistikama zauzetosti lokacije
    public class StatistikaZauzetosti
    {
        public Lokacija Lokacija { get; set; } = null!;
        public int UkupnoResursa { get; set; }
        public int ZauzetihResursa { get; set; }
        public double ProcenatZauzetosti { get; set; }

        public override string ToString() =>
            $"{Lokacija.Ime}: {ZauzetihResursa}/{UkupnoResursa} zauzeto ({ProcenatZauzetosti}%)";
    }
}