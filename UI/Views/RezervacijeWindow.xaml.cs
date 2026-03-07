using System.Windows;
using CoWorkingManager.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class RezervacijeWindow : Window
    {

        private GlavniMediator mediator;

        public RezervacijeWindow(GlavniMediator mediator, string imeLanca)
        {
            this.mediator = mediator;
            InitializeComponent();
            NazivLanca.Text = imeLanca;
        }

        private void RezervacijeWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}