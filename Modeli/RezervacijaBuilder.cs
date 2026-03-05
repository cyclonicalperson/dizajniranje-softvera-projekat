namespace CoWorkingManager.Modeli
{
    // Builder Pattern za kreiranje rezervacija
    // BUILDER obrazac — kreiranje rezervacije
    //
    // Upotreba:
    //   var rezervacija = new RezervacijaBuilder()
    //       .ZaKorisnika(korisnikId)
    //       .NaResursu(resursId)
    //       .Od(DateTime.Now.AddHours(1))
    //       .Do(DateTime.Now.AddHours(3))
    //       .Build();
    public class RezervacijaBuilder
    {
        private int? korisnikId;
        private int? resursId;
        private DateTime? pocetakVremena;
        private DateTime? krajVremena;
        private StatusRezervacije status = StatusRezervacije.Aktivna;

        // Postavlja korisnika koji pravi rezervaciju
        public RezervacijaBuilder ZaKorisnika(int korisnikId)
        {
            this.korisnikId = korisnikId;
            return this;
        }

        // Postavlja resurs koji se rezerviše
        public RezervacijaBuilder NaResursu(int resursId)
        {
            this.resursId = resursId;
            return this;
        }

        // Postavlja datum i vreme pocetka rezervacije
        public RezervacijaBuilder Od(DateTime pocetakVremena)
        {
            this.pocetakVremena = pocetakVremena;
            return this;
        }

        // Postavlja datum i vreme zavrsetka rezervacije
        public RezervacijaBuilder Do(DateTime krajVremena)
        {
            this.krajVremena = krajVremena;
            return this;
        }

        // Postavlja status rezervacije (podrazumevano: Aktivna)
        public RezervacijaBuilder SaStatusom(StatusRezervacije status)
        {
            this.status = status;
            return this;
        }

        // Gradi i vraca Rezervacija objekat
        // Baca InvalidOperationException ako neko obavezno polje nije postavljeno
        // Baca ArgumentException ako je kraj pre pocetka
        public Rezervacija Build()
        {
            if (korisnikId == null)
                throw new InvalidOperationException(
                    "Nije postavljen korisnik, pozovi ZaKorisnika() pre Build()");

            if (resursId == null)
                throw new InvalidOperationException(
                    "Nije postavljen resurs, pozovi NaResursu() pre Build()");

            if (pocetakVremena == null)
                throw new InvalidOperationException(
                    "Nije postavljeno vreme pocetka, pozovi Od() pre Build()");

            if (krajVremena == null)
                throw new InvalidOperationException(
                    "Nije postavljeno vreme zavrsetka, pozovi Do() pre Build()");

            if (krajVremena <= pocetakVremena)
                throw new ArgumentException(
                    $"Vreme završetka ({krajVremena}) mora biti posle vremena pocetka ({pocetakVremena})");

            return new Rezervacija
            {
                KorisnikId = korisnikId.Value,
                ResursId = resursId.Value,
                PocetakVreme = pocetakVremena.Value,
                KrajVreme = krajVremena.Value,
                StatusRezervacije = status
            };
        }
    }
}