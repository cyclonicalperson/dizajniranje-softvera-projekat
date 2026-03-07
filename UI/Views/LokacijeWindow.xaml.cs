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
    public partial class LokacijeWindow : Window
    {
        private GlavniMediator mediator;
        private LokacijeMediator lokacijeMediator;
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();
        private LokacijaServis lokacijaServis = new LokacijaServis(); // Pretpostavljamo servis

        List<Lokacija> Lokacije;

        public LokacijeWindow(GlavniMediator mediator, LokacijeMediator lokacijeMediator)
        {
            this.mediator = mediator;
            this.lokacijeMediator = lokacijeMediator;
            InitializeComponent();
            string[] configLines = File.ReadAllLines("config.txt");
            Lanac.Text = configLines[0];
        }

        public void Show()
        {
            Pretraga.Visibility = Visibility.Collapsed;
            Izmena.Visibility = Visibility.Collapsed;
            base.Show();
        }

        public void RefreshTable()
        {
            Lokacije = facade.Lokacije.DajSve(); // Servis sa statistikama
            TabelaLokacija.ItemsSource = null;
            TabelaLokacija.ItemsSource = Lokacije;
        }

        public bool Update(int op)
        {
            string NazivLokacije = TextBoxNazivLokacije.Text;
            string Adresa = TextBoxAdresa.Text;
            string Grad = TextBoxGrad.Text;
            string RadnoVreme = TextBoxRadnoVreme.Text;
            int MaksimalanKapacitet = 0;
            int.TryParse(TextBoxMaksimalanKapacitet.Text, out MaksimalanKapacitet);
            string Opis = TextBoxOpis.Text;

            if (string.IsNullOrWhiteSpace(NazivLokacije) || string.IsNullOrWhiteSpace(Adresa) || string.IsNullOrWhiteSpace(Grad))
                return false;

            if (op == 0) // Dodaj
            {
                return lokacijaServis.dodajLokaciju(NazivLokacije, Adresa, Grad, RadnoVreme, MaksimalanKapacitet);
            }
            else if (op == 1) // Izmeni
            {
                return lokacijaServis.izmeniLokaciju(NazivLokacije, Adresa, Grad, RadnoVreme, MaksimalanKapacitet);
            }
            else // Obrisi
            {
                return lokacijaServis.obrisiLokaciju(NazivLokacije);
            }
        }

        private void Pretraga_Click(object sender, RoutedEventArgs e)
        {
            lokacijeMediator.Notify(this, "Meni_Pretraga");
        }

        private void Izmena_Click(object sender, RoutedEventArgs e)
        {
            lokacijeMediator.Notify(this, "Meni_Izmena");
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            lokacijeMediator.Notify(this, "Dodaj");
        }

        private void Izmeni_Click(object sender, RoutedEventArgs e)
        {
            lokacijeMediator.Notify(this, "Izmeni");
        }

        private void Obrisi_Click(object sender, RoutedEventArgs e)
        {
            lokacijeMediator.Notify(this, "Obrisi");
        }

        private void GlavniMeni_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_GlavniMeni");
        }
    }
}