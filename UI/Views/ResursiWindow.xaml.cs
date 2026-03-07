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
    public partial class ResursiWindow : Window
    {
        private GlavniMediator mediator;
        private ResursiMediator resursiMediator;
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();
        private ResursServis resursServis = new ResursServis();
        private LokacijaServis lokacijaServis = new LokacijaServis();

        private string SelektovanaLokacija;
        List<Lokacija> Lokacije;
        List<Resurs> Resursi;

        public ResursiWindow(GlavniMediator mediator, ResursiMediator resursiMediator)
        {
            this.mediator = mediator;
            this.resursiMediator = resursiMediator;
            InitializeComponent();
        }

        public void Show()
        {
            Pretraga.Visibility = Visibility.Collapsed;
            Izmena.Visibility = Visibility.Collapsed;
            base.Show();
        }

        public void RefreshPretragaMeni()
        {
            SelResursi_LokCBox.Items.Clear();
            Lokacije = facade.Lokacije.DajSve();
            SelResursi_LokCBox.Items.Add("Lokacija");
            SelektovanaLokacija = "Lokacija";
            foreach (Lokacija x in Lokacije)
                SelResursi_LokCBox.Items.Add(x.Ime);
            SelResursi_LokCBox.SelectedIndex = 0;
        }

        public void RefreshTable()
        {
            if (SelektovanaLokacija == "Lokacija")
                SelektovanaLokacija = null;

            //Mora se popraviti
            //Resursi = resursServis.dajResursePoLokacijiSortiranoPoTipu(lokacijaServis.getLokacija());
            TabelaResursa.ItemsSource = null;
            TabelaResursa.ItemsSource = Resursi;
        }

        public bool Update(int op)
        {
            string Naziv = TextBoxNaziv.Text;
            string TipResursa = TextBoxTipResursa.Text;
            string Opis = TextBoxOpis.Text;
            string LokacijaID = TextBoxLokacijaID.Text;
            bool ImaProjektor = CheckBoxImaProjektor.IsChecked ?? false;
            bool ImaTV = CheckBoxImaTV.IsChecked ?? false;
            bool ImaTablu = CheckBoxImaTablu.IsChecked ?? false;
            bool ImaOnlineOpremu = CheckBoxImaOnlineOpremu.IsChecked ?? false;
            string Kapacitet = TextBoxKapacitet.Text;

            if (string.IsNullOrWhiteSpace(Naziv) || string.IsNullOrWhiteSpace(TipResursa))
                return false;

            if (op == 0) // Dodaj
            {
                return resursServis.dodajResurs(Naziv, LokacijaID, TipResursa, Opis);
            }
            else if (op == 1) // Izmeni
            {
                return resursServis.izmeniResurs(Naziv, LokacijaID, TipResursa, Opis);
            }
            else // Obrisi
            {
                return resursServis.obrisiResurs(Naziv);
            }
        }

        private void SelResursi_LokCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelektovanaLokacija = comboBox.SelectedItem.ToString();
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
    }
}