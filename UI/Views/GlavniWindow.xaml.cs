using CoWorkingManager.Mediator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Windows;

namespace CoWorkingManager.UI.Views
{
    public partial class GlavniWindow : Window
    {
        private GlavniMediator mediator;

        public GlavniWindow(GlavniMediator mediator)
        {
            this.mediator = mediator;
            InitializeComponent();
            //Dodati naziv lanca
            //Lanac.Text = 
        }


        private void Korisnici_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_Korisnike");
        }

        private void Lokacije_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_Lokacije");
        }

        private void TipoviClanstva_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_TipoveClanstva");
        }

        private void Resursi_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_Resurse");
        }

        private void Rezervacije_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_Rezervacije");
        }
    }
}