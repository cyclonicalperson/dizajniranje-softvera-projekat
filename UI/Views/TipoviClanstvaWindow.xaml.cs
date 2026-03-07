using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using CoWorkingManager.UI.Mediator;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;

namespace CoWorkingManager.UI.Views
{
    public partial class TipoviClanstvaWindow : Window
    {
        private GlavniMediator mediator;

        public TipoviClanstvaWindow(GlavniMediator mediator)
        {
            this.mediator = mediator;
            this.tipoviClanstvaMediator = tipoviClanstvaMediator;
            InitializeComponent();
        }

        private void Izmeni_Click(object sender, RoutedEventArgs e)
        {
            tipoviClanstvaMediator.Notify(this, "Izmeni");
        }

        private void Obrisi_Click(object sender, RoutedEventArgs e)
        {
            tipoviClanstvaMediator.Notify(this, "Obrisi");
        }

        private void GlavniMeni_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_GlavniMeni");
        }
    }
}