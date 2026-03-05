namespace CoWorkingManager.Modeli
{
    // Factory Pattern za pravljenje resursa
    // Daje jasno definisane funkcije za pravljenje stolova, sala i kancelarija
    public static class ResursFactory
    {
        // Kreira radno mesto (hot desk ili dedicated desk)
        // Polja opreme i kapaciteta su uvek NULL — nisu relevantna za sto
        public static Resurs KreirajSto(
            int lokacijaId,
            string ime,
            PodtipStola podtip,
            string? opis = null)
        {
            return new Resurs
            {
                LokacijaId = lokacijaId,
                Ime = ime,
                TipResursa = TipResursa.Sto,
                Opis = opis,
                PodtipStola = podtip,

                // Polja koja nisu relevantna za sto — NULL
                Kapacitet = null,
                ImaProjektor = null,
                ImaTV = null,
                ImaTablu = null,
                ImaOnlineOpremu = null
            };
        }

        // Kreira salu za sastanke
        // Kapacitet i sva polja opreme su obavezna
        // PodtipStola je uvek NULL
        public static Resurs KreirajSalu(
            int lokacijaId,
            string ime,
            int kapacitet,
            bool imaProjektor,
            bool imaTV,
            bool imaTablu,
            bool imaOnlineOpremu,
            string? opis = null)
        {
            if (kapacitet <= 0)
                throw new ArgumentException("Kapacitet sale mora biti veći od 0", nameof(kapacitet));

            return new Resurs
            {
                LokacijaId = lokacijaId,
                Ime = ime,
                TipResursa = TipResursa.Sala,
                Opis = opis,

                // Polje koje nije relevantno za salu — NULL
                PodtipStola = null,

                // Obavezna polja za salu
                Kapacitet = kapacitet,
                ImaProjektor = imaProjektor,
                ImaTV = imaTV,
                ImaTablu = imaTablu,
                ImaOnlineOpremu = imaOnlineOpremu
            };
        }

        // Kreira privatnu kancelariju
        // Kapacitet je obavezan, oprema je opcionalna (default false)
        // PodtipStola je uvek NULL
        public static Resurs KreirajPrivatnuKancelariju(
            int lokacijaId,
            string ime,
            int kapacitet,
            bool imaProjektor = false,
            bool imaTV = false,
            bool imaTablu = false,
            bool imaOnlineOpremu = false,
            string? opis = null)
        {
            if (kapacitet <= 0)
                throw new ArgumentException("Kapacitet kancelarije mora biti veći od 0.", nameof(kapacitet));

            return new Resurs
            {
                LokacijaId = lokacijaId,
                Ime = ime,
                TipResursa = TipResursa.PrivatnaKancelarija,
                Opis = opis,

                // Polje koje nije relevantno za kancelariju — NULL
                PodtipStola = null,

                // Polja za kancelariju
                Kapacitet = kapacitet,
                ImaProjektor = imaProjektor,
                ImaTV = imaTV,
                ImaTablu = imaTablu,
                ImaOnlineOpremu = imaOnlineOpremu
            };
        }
    }
}