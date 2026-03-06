using CoWorkingManager.Modeli;
using CoWorkingManager.UI.Views;

namespace CoWorkingManager.Mediator
{
    public class GlavniMediator : IMediator
    {
        private GlavniWindow glavna;
        private KorisniciWindow korisnici;
        private LokacijeWindow lokacije;
        private TipoviClanstvaWindow tipoviClanstva;
        private ResursiWindow resursi;
        private RezervacijeWindow rezervacije;

        public void SetMain(GlavniWindow w) => glavna = w;
        public void SetUsers(KorisniciWindow w) => korisnici = w;
        public void SetLocations(LokacijeWindow w) => lokacije = w;
        public void SetMemberships(TipoviClanstvaWindow w) => tipoviClanstva = w;
        public void SetResources(ResursiWindow w) => resursi = w;
        public void SetReservations(RezervacijeWindow w) => rezervacije = w;

        public void Notify(object sender, string eventCode)
        {
            switch (eventCode)
            {
                case "OPEN_USERS":
                    korisnici.Show();
                    break;

                case "OPEN_LOCATIONS":
                    lokacije.Show();
                    break;

                case "OPEN_MEMBERSHIP":
                    tipoviClanstva.Show();
                    break;

                case "OPEN_RESOURCES":
                    resursi.Show();
                    break;

                case "OPEN_RESERVATIONS":
                    rezervacije.Show();
                    break;
            }
        }
    }
}