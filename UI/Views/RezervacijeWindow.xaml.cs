using System.Windows;
using CoWorkingManager.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class RezervacijeWindow : Window
    {

        private GlavniMediator mediator;

        public RezervacijeWindow(GlavniMediator mediator)
        {
            this.mediator = mediator;
            InitializeComponent();
        }

        private void RezervacijeWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}