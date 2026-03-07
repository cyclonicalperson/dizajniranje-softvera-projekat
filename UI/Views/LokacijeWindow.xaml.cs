using System.Windows;
using CoWorkingManager.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class LokacijeWindow : Window
    {
        private GlavniMediator mediator;

        public LokacijeWindow(GlavniMediator mediator, string imeLanca)
        {
            this.mediator = mediator;
            InitializeComponent();
            NazivLanca.Text = imeLanca;
        }

        private void LokacijeWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
