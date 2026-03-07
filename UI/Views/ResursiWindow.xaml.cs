using System.Windows;
using CoWorkingManager.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class ResursiWindow : Window
    {
        private GlavniMediator mediator;

        public ResursiWindow(GlavniMediator mediator, string imeLanca)
        {
            this.mediator = mediator;
            InitializeComponent();
            NazivLanca.Text = imeLanca;
        }

        private void ResursiWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
