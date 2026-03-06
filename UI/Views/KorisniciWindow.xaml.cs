using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using CoWorkingManager.Podaci.Repozitorijumi;
using System.Windows;
using System.Windows.Controls;

namespace CoWorkingManager.UI.Views
{
    public partial class KorisniciWindow : Window
    {
        private GlavniMediator mediator;
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();
        KorisnikServis korisnikServis = new KorisnikServis();
        string SelektovanaLokacija;
        string SelektovanTipClanstva;
        string SelektovanStatusaNaloga;
        List<Lokacija> Lokacije;
        List<TipClanstva> TipoviClanstva;
        //List<> StatusiNaloga;
        List<Korisnik> Korisnici;

        public KorisniciWindow(GlavniMediator mediator)
        {
            this.mediator = mediator;
            facade = CoworkingFasada.DajInstancu();
            InitializeComponent();
        }

        public void Show()
        {
            

            base.Show();
        }

        //Osvezava sva polja sa izborom
        public void RefreshCBoxes()
        {
            SelKorisnici_LokCBox.Items.Clear();
            SelKorisnici_TipClCBox.Items.Clear();
            SelKorisnici_StatusCBox.Items.Clear();

            Lokacije = facade.Lokacije.DajSve();
            SelKorisnici_LokCBox.Items.Add("Lokacija");
            SelektovanaLokacija = "Lokacija";
            foreach(Lokacija x in Lokacije)
                SelKorisnici_LokCBox.Items.Add(x.Ime);

            TipoviClanstva = facade.TipoviClanstva.DajSve();
            SelKorisnici_TipClCBox.Items.Add("TipClasntva");
            SelektovanTipClanstva = "TipClasntva";
            foreach (TipClanstva x in TipoviClanstva)
                SelKorisnici_LokCBox.Items.Add(x.Ime);


            SelKorisnici_StatusCBox.Items.Add("StatusNaloga");
            SelektovanStatusaNaloga = "StatusNaloga";
            //foreach()
            //SelKorisnici_StatusCBox.Items.Add(x);
            SelKorisnici_LokCBox.SelectedIndex = 0;
            SelKorisnici_TipClCBox.SelectedIndex = 0;
            SelKorisnici_StatusCBox.SelectedIndex = 0;

        }

        //Osvezava prikazanu tabelu
        public void RefreshTable()
        {
            ListBoxIme.Items.Clear();
            ListBoxPrezime.Items.Clear();
            ListBoxEmail.Items.Clear();
            ListBoxBrojTelefona.Items.Clear();
            ListBoxTipClanstva.Items.Clear();
            ListBoxDatumPocetkaClanstva.Items.Clear();
            ListBoxDatumIstekaClanstva.Items.Clear();
            ListBoxStatusNaloga.Items.Clear();
            //Korisnici = korisnikServis.dajKorisnike();
            foreach (Korisnik x in Korisnici)
            {
                ListBoxIme.Items.Add(x.Ime);
                ListBoxPrezime.Items.Add(x.Prezime);
                ListBoxEmail.Items.Add(x.Email);
                ListBoxBrojTelefona.Items.Add(x.Telefon);
                ListBoxTipClanstva.Items.Add(x.TipClanstva);
                ListBoxDatumPocetkaClanstva.Items.Add(x.DatumPocetkaClanstva);
                ListBoxDatumIstekaClanstva.Items.Add(x.DatumKrajaClanstva);
                ListBoxStatusNaloga.Items.Add(x.StatusNaloga);
            }
        }

        private void SelKorisnici_LokCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            SelektovanaLokacija = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
            MessageBox.Show($"Selected item: {SelektovanaLokacija}");
        }
        private void SelKorisnici_TipClCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            SelektovanTipClanstva = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
            MessageBox.Show($"Selected item: {SelektovanTipClanstva}");
        }
        private void SelKorisnici_StatusCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string SelektovanStatusaNaloga = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
            MessageBox.Show($"Selected item: {SelektovanStatusaNaloga}");
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
