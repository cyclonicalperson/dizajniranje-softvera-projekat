using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.UI.Mediator;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace CoWorkingManager.UI.Views
{
    public partial class RezervacijeWindow : Window
    {
        private GlavniMediator mediator;
        private RezervacijeMediator rezervacijeMediator;
        private RezervacijaServisProxy rezervacijaServisProxy = new RezervacijaServisProxy(new RezervacijaServis());
        private LokacijaServisProxy lokacijaServisProxy = new LokacijaServisProxy(new LokacijaServis());
        private KorisnikServisProxy korisnikServisProxy = new KorisnikServisProxy(new KorisnikServis());
        private string SelektovaniKorisnik;
        private DateTime? SelektovaniDan;
        private string SelektovanaLokacija;

        List<Korisnik> Korisnici;
        List<Lokacija> Lokacije;
        List<Rezervacija> Rezervacije;

        public RezervacijeWindow(GlavniMediator mediator, RezervacijeMediator rezervacijeMediator)
        {
            this.mediator = mediator;
            this.rezervacijeMediator = rezervacijeMediator;
            InitializeComponent();
            string[] configLines = File.ReadAllLines("config.txt");
            Lanac.Text = configLines[0];
        }

        public void Show()
        {
            PretragaKorisnik.Visibility = Visibility.Collapsed;
            PretragaDanLokacija.Visibility = Visibility.Collapsed;
            base.Show();
        }

        public void RefreshPretragaKorisnikMeni()
        {
            SelRezervacije_KorisnikCBox.Items.Clear();
            Korisnici = korisnikServisProxy.dajSve();
            SelRezervacije_KorisnikCBox.Items.Add("Korisnik");
            SelektovaniKorisnik = "Korisnik";
            foreach (Korisnik x in Korisnici)
                SelRezervacije_KorisnikCBox.Items.Add(x.Ime + " " + x.Prezime);
            SelRezervacije_KorisnikCBox.SelectedIndex = 0;
        }

        public void RefreshPretragaDanLokacijaMeni()
        {
            SelRezervacije_LokacijaCBox.Items.Clear();
            Lokacije = lokacijaServisProxy.dajSve();
            SelRezervacije_LokacijaCBox.Items.Add("Lokacija");
            SelektovanaLokacija = "Lokacija";
            foreach (Lokacija x in Lokacije)
                SelRezervacije_LokacijaCBox.Items.Add(x.Ime);
            SelRezervacije_LokacijaCBox.SelectedIndex = 0;
        }

        public void RefreshTableKorisnik()
        {
            if (SelektovaniKorisnik == "Korisnik")
                SelektovaniKorisnik = null;

            Rezervacije = rezervacijaServisProxy.dajRezervacijeKorisnika(SelektovaniKorisnik);
            TabelaRezervacijaKorisnik.ItemsSource = null;
            TabelaRezervacijaKorisnik.ItemsSource = Rezervacije;
        }

        public void RefreshTableDanLokacija()
        {
            if (SelektovanaLokacija == "Lokacija")
                SelektovanaLokacija = null;

            Rezervacije = rezervacijaServisProxy.dajRezervacijePoLokacijiIDanu(SelektovanaLokacija, SelektovaniDan);
            TabelaRezervacijaDanLokacija.ItemsSource = null;
            TabelaRezervacijaDanLokacija.ItemsSource = Rezervacije;
        }

        private void SelRezervacije_KorisnikCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelektovaniKorisnik = comboBox.SelectedItem.ToString();
            RefreshTableKorisnik();
        }

        private void PretragaKorisnik_Click(object sender, RoutedEventArgs e)
        {
            rezervacijeMediator.Notify(this, "Meni_PretragaKorisnik");
        }

        private void PretragaDanLokacija_Click(object sender, RoutedEventArgs e)
        {
            rezervacijeMediator.Notify(this, "Meni_PretragaDanLokacija");
        }

        private void Upravljaj_Click(object sender, RoutedEventArgs e)
        {

            RezervacijeDialogMediator dialogMediator = new RezervacijeDialogMediator();
            RezervacijeDialog dialog = new RezervacijeDialog(dialogMediator);
            dialog.Show();
        }

        private void PretraziDanLokacija_Click(object sender, RoutedEventArgs e)
        {
            SelektovaniDan = SelRezervacije_Dan.SelectedDate;
            if (SelRezervacije_LokacijaCBox.SelectedItem != null)
                SelektovanaLokacija = SelRezervacije_LokacijaCBox.SelectedItem.ToString();
            RefreshTableDanLokacija();
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