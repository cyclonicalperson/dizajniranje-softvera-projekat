using System.Windows;
using CoWorkingManager.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class TipoviClanstvaWindow : Window
    {
        private GlavniMediator mediator;

        public TipoviClanstvaWindow(GlavniMediator mediator)
        {
            this.mediator = mediator;
            InitializeComponent();
        }
    }
}
