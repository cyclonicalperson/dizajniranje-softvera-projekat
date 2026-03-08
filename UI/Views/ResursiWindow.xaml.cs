using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.UI.Mediator;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace CoWorkingManager.UI.Views
{
    public partial class ResursiWindow : Window
    {
        private GlavniMediator mediator;
        private ResursiMediator resursiMediator;
        private ResursServisProxy resursServisProxy = new ResursServisProxy(new ResursServis());
        private LokacijaServisProxy lokacijaServisProxy = new LokacijaServisProxy(new LokacijaServis());

        private string? SelektovanaLokacija;
        List<Lokacija>? Lokacije;
        List<Resurs>? Resursi;

        public ResursiWindow(GlavniMediator mediator, ResursiMediator resursiMediator)
        {
            this.mediator = mediator;
            this.resursiMediator = resursiMediator;
            InitializeComponent();
            string[] configLines = File.ReadAllLines("config.txt");
            Lanac.Text = configLines[0];
        }

        // Standardno otvaranje bez predodabrane lokacije (npr. iz glavnog menija)
        public new void Show()
        {
            SelektovanaLokacija = null;
            Pretraga.Visibility = Visibility.Collapsed;
            Izmena.Visibility = Visibility.Collapsed;
            base.Show();
        }

        // Otvaranje sa već izabranom lokacijom — poziva se iz GlavniMediator
        // kad korisnik klikne "Izaberi" u LokacijeWindow
        public void ShowSaLokacijom(string nazivLokacije)
        {
            // Popuni ComboBox lokacijama
            RefreshPretragaMeni();

            SelektovanaLokacija = nazivLokacije;

            // Prikazi panel pretrage i postavi ComboBox na izabranu lokaciju
            Pretraga.Visibility = Visibility.Visible;
            Izmena.Visibility = Visibility.Collapsed;
            Uspesno.Visibility = Visibility.Collapsed;
            Neuspesno.Visibility = Visibility.Collapsed;

            // Selektuj ispravnu lokaciju u ComboBoxu
            foreach (var item in SelResursi_LokCBox.Items)
            {
                if (item?.ToString() == SelektovanaLokacija)
                {
                    SelResursi_LokCBox.SelectedItem = item;
                    break;
                }
            }

            // Prikazi resurse izabrane lokacije
            RefreshTable();

            // Prikazi header sa nazivom aktivne lokacije
            AktivnaLokacijaLabel.Text = $"Aktivna lokacija: {SelektovanaLokacija}";
            AktivnaLokacijaLabel.Visibility = Visibility.Visible;

            base.Show();
        }

        public void RefreshPretragaMeni()
        {
            SelResursi_LokCBox.Items.Clear();
            Lokacije = lokacijaServisProxy.dajSve();
            SelResursi_LokCBox.Items.Add("Lokacija");
            if (SelektovanaLokacija == null)
                SelektovanaLokacija = "Lokacija";
            foreach (Lokacija x in Lokacije)
                SelResursi_LokCBox.Items.Add(x.Ime);
            SelResursi_LokCBox.SelectedIndex = 0;
        }

        public void RefreshTable()
        {
            string? filtriranaLokacija = SelektovanaLokacija == "Lokacija" ? null : SelektovanaLokacija;

            Resursi = resursServisProxy.dajResursePoLokacijiSortiranoPoTipu(filtriranaLokacija);
            TabelaResursa.ItemsSource = null;
            TabelaResursa.ItemsSource = Resursi;
        }

        public bool Update(int op)
        {
            string? Naziv = TextBoxNaziv.Text;
            string? LokacijaIme = TextBoxLokacijaID.Text;
            string? TipResursa = TextBoxTipResursa.Text;
            string? Opis = string.IsNullOrWhiteSpace(TextBoxOpis.Text) ? null : TextBoxOpis.Text;
            string? PodtipStola = string.IsNullOrWhiteSpace(TextBoxPodtipStola.Text) ? null : TextBoxPodtipStola.Text;
            int? Kapacitet = int.TryParse(TextBoxKapacitet.Text, out int value) ? value : null;

            bool? ImaProjektor = CheckBoxImaProjektor.IsChecked;
            bool? ImaTV = CheckBoxImaTV.IsChecked;
            bool? ImaTablu = CheckBoxImaTablu.IsChecked;
            bool? ImaOnlineOpremu = CheckBoxImaOnlineOpremu.IsChecked;

            if (string.IsNullOrWhiteSpace(Naziv) || string.IsNullOrWhiteSpace(TipResursa))
                return false;

            if (op == 0) // Dodaj
            {
                if (string.IsNullOrWhiteSpace(LokacijaIme))
                    return false;
                return resursServisProxy.kreirajResurs(Naziv, LokacijaIme, TipResursa, Opis, PodtipStola, Kapacitet, ImaProjektor, ImaTV, ImaTablu, ImaOnlineOpremu);
            }
            else if (op == 1) // Izmeni
            {
                return resursServisProxy.izmeniResurs(Naziv, LokacijaIme, TipResursa, Opis, PodtipStola, Kapacitet, ImaProjektor, ImaTV, ImaTablu, ImaOnlineOpremu);
            }
            else // Obrisi
            {
                return resursServisProxy.otkaziResurs(Naziv);
            }
        }

        private void SelResursi_LokCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelektovanaLokacija = comboBox.SelectedItem.ToString();

            // Ažuriraj label aktivne lokacije
            if (SelektovanaLokacija != null && SelektovanaLokacija != "Lokacija")
            {
                AktivnaLokacijaLabel.Text = $"Aktivna lokacija: {SelektovanaLokacija}";
                AktivnaLokacijaLabel.Visibility = Visibility.Visible;
            }
            else
            {
                AktivnaLokacijaLabel.Visibility = Visibility.Collapsed;
            }

            RefreshTable();
        }

        private void Pretraga_Click(object sender, RoutedEventArgs e)
        {
            resursiMediator.Notify(this, "Meni_Pretraga");
        }

        private void Izmena_Click(object sender, RoutedEventArgs e)
        {
            resursiMediator.Notify(this, "Meni_Izmena");
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            resursiMediator.Notify(this, "Dodaj");
        }

        private void Izmeni_Click(object sender, RoutedEventArgs e)
        {
            resursiMediator.Notify(this, "Izmeni");
        }

        private void Obrisi_Click(object sender, RoutedEventArgs e)
        {
            resursiMediator.Notify(this, "Obrisi");
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