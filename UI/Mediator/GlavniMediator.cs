using CoWorkingManager.UI.Views;

namespace CoWorkingManager.Mediator
{
    public class GlavniMediator : IMediator
    {
        private GlavniWindow main;
        private KorisniciWindow users;
        private LokacijeWindow locations;
        private TipoviClanstvaWindow memberships;
        private ResursiWindow resources;
        private RezervacijeWindow reservations;

        public void SetMain(GlavniWindow w) => main = w;
        public void SetUsers(KorisniciWindow w) => users = w;
        public void SetLocations(LokacijeWindow w) => locations = w;
        public void SetMemberships(TipoviClanstvaWindow w) => memberships = w;
        public void SetResources(ResursiWindow w) => resources = w;
        public void SetReservations(RezervacijeWindow w) => reservations = w;

        public void Notify(object sender, string eventCode)
        {
            switch (eventCode)
            {
                case "OPEN_USERS":
                    users.Show();
                    break;

                case "OPEN_LOCATIONS":
                    locations.Show();
                    break;

                case "OPEN_MEMBERSHIP":
                    memberships.Show();
                    break;

                case "OPEN_RESOURCES":
                    resources.Show();
                    break;

                case "OPEN_RESERVATIONS":
                    reservations.Show();
                    break;
            }
        }
    }
}