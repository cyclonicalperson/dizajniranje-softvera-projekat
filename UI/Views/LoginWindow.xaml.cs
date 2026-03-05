using System.Windows;
using CoWorkingManager.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            //InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var mediator = new GlavniMediator();

            /*var main = new GlavniWindow(mediator);
            var users = new UsersWindow(mediator);
            var locations = new LocationsWindow(mediator);
            var memberships = new MembershipWindow(mediator);
            var resources = new ResourcesWindow(mediator);
            var reservations = new ReservationsWindow(mediator);

            mediator.SetMain(main);
            mediator.SetUsers(users);
            mediator.SetLocations(locations);
            mediator.SetMemberships(memberships);
            mediator.SetResources(resources);
            mediator.SetReservations(reservations);

            main.Show();*/
            this.Close();
        }
    }
}