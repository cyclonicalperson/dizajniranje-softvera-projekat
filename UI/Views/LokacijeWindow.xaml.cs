using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.UI.Mediator;
using System.Windows;
using System.IO;
using CoWorkingManager.Podaci.Repozitorijumi;

namespace CoWorkingManager.UI.Views
{
    public partial class LokacijeWindow : Window
    {
        private GlavniMediator mediator;
        private LokacijeMediator lokacijeMediator;
        private LokacijaServisProxy lokacijaServisProxy = new LokacijaServisProxy(new LokacijaServis());

        List<StatistikaZauzetosti> StatistikeZauzetosti;
        List <Lokacija> Lokacije;

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
            StatistikeZauzetosti = lokacijaServisProxy.DajStatistikuZauzetostiZaSve(DateTime.Today);
            Lokacije = lokacijaServisProxy.dajSve();
            TabelaLokacija.ItemsSource = null;
            TabelaLokacija.ItemsSource = StatistikeZauzetosti;
        }

        public bool Update(int op)
        {
            string? NazivLokacije = TextBoxNazivLokacije.Text;
            string? Adresa = TextBoxAdresa.Text;
            string? Grad = TextBoxGrad.Text;
            string? RadnoVreme = TextBoxRadnoVreme.Text;
            int? MaksimalanKapacitet = int.TryParse(TextBoxMaksimalanKapacitet.Text, out int value) ? value : null;

            if (string.IsNullOrWhiteSpace(NazivLokacije))
                return false;

            if (op == 0) // Dodaj
            {
                if (string.IsNullOrWhiteSpace(NazivLokacije) || string.IsNullOrWhiteSpace(Adresa) || string.IsNullOrWhiteSpace(Grad) 
                    || string.IsNullOrWhiteSpace(RadnoVreme) || MaksimalanKapacitet == null)
                    return false;
                return lokacijaServisProxy.dodajLokaciju(NazivLokacije, Adresa, Grad, RadnoVreme, (int)MaksimalanKapacitet);
            }
            else if (op == 1) // Izmeni
            {
                return lokacijaServisProxy.izmeniLokaciju(NazivLokacije, Adresa, Grad, RadnoVreme, MaksimalanKapacitet);
            }
            else // Obrisi
            {
                return lokacijaServisProxy.obrisiLokaciju(NazivLokacije);
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

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}