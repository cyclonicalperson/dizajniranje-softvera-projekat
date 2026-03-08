using CoWorkingManager.Mediator;
using CoWorkingManager.Logika.Servisi;
using System.Windows;
using System.IO;
using System;

namespace CoWorkingManager.UI.Views
{
    public partial class GlavniWindow : Window
    {
        private GlavniMediator mediator;

        public GlavniWindow(GlavniMediator mediator, string imeLanca)
        {
            this.mediator = mediator;
            InitializeComponent();
            // Učitaj naziv lanca iz config.txt
            string[] configLines = File.ReadAllLines("config.txt");
            Lanac.Text = configLines[0]; // Prva linija je naziv lanca
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