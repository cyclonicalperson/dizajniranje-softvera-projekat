using CoWorkingManager.Mediator;
using System.Windows;
using System.Windows.Controls;

namespace CoWorkingManager.UI.Views
{
    public partial class KorisniciWindow : Window
    {
        private GlavniMediator mediator;
        string SelektovanaLokacija;
        string SelektovanTipClanstva;
        string SelektovanStatusaNaloga;

        public KorisniciWindow(GlavniMediator mediator)
        {
            this.mediator = mediator;
            SelKorisnici_LokCBox.Items.Add("Lokacija");
            SelektovanaLokacija = "Lokacija";
            SelKorisnici_TipClCBox.Items.Add("TipClasntva");
            SelektovanTipClanstva = "TipClasntva";
            SelKorisnici_StatusCBox.Items.Add("StatusNaloga");
            SelektovanStatusaNaloga = "StatusNaloga";
            InitializeComponent();
        }

        public void Show()
        {
            SelKorisnici_LokCBox.SelectedIndex = 0;
            SelKorisnici_TipClCBox.SelectedIndex = 0;
            SelKorisnici_StatusCBox.SelectedIndex = 0;
            base.Show();
        }

        private void SelKorisnici_LokCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selectedItem = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
            MessageBox.Show($"Selected item: {selectedItem}");
        }
        private void SelKorisnici_TipClCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selectedItem = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
            MessageBox.Show($"Selected item: {selectedItem}");
        }
        private void SelKorisnici_StatusCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selectedItem = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
            MessageBox.Show($"Selected item: {selectedItem}");
        }

        //Funkcija koja vrsi pretragu
        private void Pretrazi_Click(object sender, RoutedEventArgs e)
        {
            //mediator.Notify(this, "Otvori_Korisnike");
        }
        
        //Funkcija za povratak na Glavni Meni
        private void GlavniMeni_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_GlavniMeni");
        }
    }
}
