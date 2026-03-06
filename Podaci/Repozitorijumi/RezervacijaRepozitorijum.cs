using Microsoft.EntityFrameworkCore;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Podaci.Repozitorijumi
{
    // Repozitorijum za upravljanje rezervacijama
    // Pruza CRUD, proveru preklapanja, filtriranje i upite za izvestaje
    public class RezervacijaRepozitorijum
    {
        private readonly KontekstBaze kontekst;

        public RezervacijaRepozitorijum(KontekstBaze kontekst)
        {
            this.kontekst = kontekst;
        }

        // Vraca sve rezervacije sa korisnicima i resursima
        public List<Rezervacija> DajSve()
        {
            return kontekst.Rezervacije
                .Include(r => r.Korisnik).ThenInclude(k => k.TipClanstva)
                .Include(r => r.Resurs).ThenInclude(res => res.Lokacija)
                .ToList();
        }

        // Vraca rezervaciju po ID-u, ili null
        public Rezervacija? DajPoId(int id)
        {
            return kontekst.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Resurs).ThenInclude(res => res.Lokacija)
                .FirstOrDefault(r => r.Id == id);
        }

        // Vraca sve rezervacije korisnika od najnovije
        public List<Rezervacija> DajPoKorisniku(int korisnikId)
        {
            return kontekst.Rezervacije
                .Include(r => r.Resurs).ThenInclude(res => res.Lokacija)
                .Where(r => r.KorisnikId == korisnikId)
                .OrderByDescending(r => r.PocetakVreme)
                .ToList();
        }

        // Vraca sve rezervacije za datu lokaciju na dati dan
        // Sortira po vremenu pocetka, ne ukljucuje otkazane
        public List<Rezervacija> DajPoLokacijiIDanu(int lokacijaId, DateTime datum)
        {
            var pocetakDana = datum.Date;
            var krajDana = datum.Date.AddDays(1);

            return kontekst.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Resurs).ThenInclude(res => res.Lokacija)
                .Where(r => r.Resurs.LokacijaId == lokacijaId
                         && r.PocetakVreme < krajDana
                         && r.KrajVreme > pocetakDana
                         && r.StatusRezervacije != StatusRezervacije.Otkazana)
                .OrderBy(r => r.PocetakVreme)
                .ToList();
        }

        // Proverava da li se trazeni termin preklapa sa postojećim rezervacijama za isti resurs
        // Otkazane rezervacije se ignorisu
        public bool PostojiPreklapanje(int resursId, DateTime pocetak, DateTime kraj, int? excludeId = null)
        {
            var upit = kontekst.Rezervacije
                .Where(r => r.ResursId == resursId
                         && r.StatusRezervacije != StatusRezervacije.Otkazana
                         && r.PocetakVreme < kraj
                         && r.KrajVreme > pocetak);

            if (excludeId.HasValue)
                upit = upit.Where(r => r.Id != excludeId.Value);

            return upit.Any();
        }

        // Vraca rezervacije za mesečni CSV izveštaj
        // Vraca sve osim otkazanih, sa punim podacima o korisniku i resursu
        public List<Rezervacija> DajZaMesecniIzvestaj(int godina, int mesec)
        {
            return kontekst.Rezervacije
                .Include(r => r.Korisnik).ThenInclude(k => k.TipClanstva)
                .Include(r => r.Resurs).ThenInclude(res => res.Lokacija)
                .Where(r => r.PocetakVreme.Year == godina
                         && r.PocetakVreme.Month == mesec
                         && r.StatusRezervacije != StatusRezervacije.Otkazana)
                .OrderBy(r => r.PocetakVreme)
                .ToList();
        }

        // Dodaje novu rezervaciju
        // Vraca false ako postoji preklapanje termina za isti resurs
        public bool Dodaj(Rezervacija rezervacija)
        {
            if (PostojiPreklapanje(rezervacija.ResursId, rezervacija.PocetakVreme, rezervacija.KrajVreme))
                return false;

            kontekst.Rezervacije.Add(rezervacija);
            kontekst.SaveChanges();
            return true;
        }



        // Menja termin ili resurs postojeće rezervacije
        // Vraca false ako rezervacija ne postoji, nije aktivna,
        // ili novi termin se preklapa sa drugom rezervacijom
        public bool Azuriraj(Rezervacija rezervacija)
        {
            var uBazi = kontekst.Rezervacije.Find(rezervacija.Id);
            if (uBazi == null)
                return false;

            // Izmena je dozvoljena samo za aktivne rezervacije
            if (uBazi.StatusRezervacije != StatusRezervacije.Aktivna)
                return false;

            // Proverava preklapanje, iskljucujući samu ovu rezervaciju
            if (PostojiPreklapanje(
                    rezervacija.ResursId,
                    rezervacija.PocetakVreme,
                    rezervacija.KrajVreme,
                    excludeId: rezervacija.Id))
                return false;

            kontekst.Rezervacije.Update(rezervacija);
            kontekst.SaveChanges();
            return true;
        }

        // Otkazuje rezervaciju postavljanjem statusa na Otkazana
        // Zapis ostaje u bazi radi istorije
        // Vraca false ako rezervacija ne postoji ili je vec otkazana/zavrsena
        public bool Otkazi(int id)
        {
            var rezervacija = kontekst.Rezervacije.Find(id);
            if (rezervacija == null)
                return false;

            if (rezervacija.StatusRezervacije != StatusRezervacije.Aktivna)
                return false;

            rezervacija.StatusRezervacije = StatusRezervacije.Otkazana;
            kontekst.SaveChanges();
            return true;
        }

        // Trajno brise rezervaciju iz baze
        // Vraca false ako rezervacija ne postoji
        public bool Obrisi(int id)
        {
            var rezervacija = kontekst.Rezervacije.Find(id);
            if (rezervacija == null)
                return false;

            kontekst.Rezervacije.Remove(rezervacija);
            kontekst.SaveChanges();
            return true;
        }

        // Automatski oznacava prosle aktivne rezervacije kao zavrsene
        // !!Treba pozivati pri pokretanju aplikacije!!
        public void ObeleziZavrseneRezervacije()
        {
            var sada = DateTime.Now;
            var zaZavrsiti = kontekst.Rezervacije
                .Where(r => r.StatusRezervacije == StatusRezervacije.Aktivna
                         && r.KrajVreme <= sada)
                .ToList();

            foreach (var r in zaZavrsiti)
                r.StatusRezervacije = StatusRezervacije.Zavrsena;

            if (zaZavrsiti.Count > 0)
                kontekst.SaveChanges();
        }
    }
}