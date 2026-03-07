using System.Windows;
using CoWorkingManager.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class LokacijeWindow : Window
    {
        private GlavniMediator mediator;

        public LokacijeWindow(GlavniMediator mediator)
        {
            this.mediator = mediator;
            InitializeComponent();
        }
    }
}
