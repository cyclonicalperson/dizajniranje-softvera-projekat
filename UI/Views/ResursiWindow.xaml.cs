using System.Windows;
using CoWorkingManager.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class ResursiWindow : Window
    {
        private GlavniMediator mediator;

        public ResursiWindow(GlavniMediator mediator)
        {
            this.mediator = mediator;
            InitializeComponent();
        }
    }
}
