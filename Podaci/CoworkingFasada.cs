using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci.Repozitorijumi;

namespace CoWorkingManager.Podaci
{
    // Fasada Pattern za komunikacije aplikacije sa repozitorijumima
    //
    // Upotreba:
    //   var facade = CoWorkingFacade.DajInstancu();      // u logici/UI — bez konekcionog stringa
    //   var facade = CoWorkingFacade.DajInstancu(ks);    // pri inicijalizaciji (Program.cs)
    //
    //   facade.Korisnici.DajSve();
    //   facade.Rezervacije.PostojiPreklapanje(...);
    //   facade.Resursi.DajDostupneResurse(...);
    public class CoworkingFasada
    {
        // ── Singleton instanca ────────────────────────────────────────────
        // Facade drzi jednu instancu sebe — KontekstBaze je vec Singleton,
        // ali Facade garantuje i da se repozitorijumi ne kreiraju višestruko

        private static CoworkingFasada? instanca;
        private static readonly object katanac = new();

        // ── Repozitorijumi — jedine javne tačke pristupa podacima ─────────

        public TipClanstvaRepozitorijum TipoviClanstva { get; }
        public LokacijaRepozitorijum Lokacije { get; }
        public KorisnikRepozitorijum Korisnici { get; }
        public ResursRepozitorijum Resursi { get; }
        public RezervacijaRepozitorijum Rezervacije { get; }
        public AdministratorRepozitorijum Administratori { get; }

        // ── Privatni konstruktor — Facade se ne pravi direktno ────────────

        private CoworkingFasada(KontekstBaze kontekst)
        {
            TipoviClanstva = new TipClanstvaRepozitorijum(kontekst);
            Lokacije = new LokacijaRepozitorijum(kontekst);
            Korisnici = new KorisnikRepozitorijum(kontekst);
            Resursi = new ResursRepozitorijum(kontekst);
            Rezervacije = new RezervacijaRepozitorijum(kontekst);
            Administratori = new AdministratorRepozitorijum(kontekst);
        }

        // ── Inicijalizacija — poziva se jednom pri pokretanju aplikacije ──

        // Inicijalizuje Facade sa konekcionim stringom
        // Mora biti pozvan pre prvog DajInstancu() bez argumenta
        public static CoworkingFasada Inicijalizuj(string konekcioniString)
        {
            lock (katanac)
            {
                if (instanca == null)
                {
                    var kontekst = KontekstBaze.DajInstancu(konekcioniString);
                    instanca = new CoworkingFasada(kontekst);
                }
            }
            return instanca;
        }

        // Vraća vec inicijalizovanu instancu
        // Baca InvalidOperationException ako Inicijalizuj() nije pozvan pre ovoga
        public static CoworkingFasada DajInstancu()
        {
            if (instanca == null)
                throw new InvalidOperationException(
                    "CoworkingFasada nije inicijalizovana " +
                    "Pozovi CoWorkingFacade.Inicijalizuj(konekcioniString) pri pokretanju aplikacije");

            return instanca;
        }

        // Resetuje Facade za testiranje ili reinicijalizaciju
        public static void Reset()
        {
            lock (katanac)
            {
                KontekstBaze.ResetInstance();
                instanca = null;
            }
        }
    }
}