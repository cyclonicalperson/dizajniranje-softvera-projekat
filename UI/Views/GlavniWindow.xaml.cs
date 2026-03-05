using CoWorkingManager.Mediator;
using System.Windows;

namespace CoWorkingManager.UI.Views
{
    public partial class GlavniWindow : Window
    {
        private IMediator mediator;

        public GlavniWindow(IMediator mediator)
        {
            //InitializeComponent();
            this.mediator = mediator;
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "OPEN_USERS");
        }

        private void Locations_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "OPEN_LOCATIONS");
        }

        private void Membership_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "OPEN_MEMBERSHIP");
        }

        private void Resources_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "OPEN_RESOURCES");
        }

        private void Reservations_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "OPEN_RESERVATIONS");
        }
    }
}