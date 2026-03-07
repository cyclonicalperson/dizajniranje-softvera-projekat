using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using CoWorkingManager.UI.Mediator;
using System.Windows;
using System.Windows.Controls;

namespace CoWorkingManager.UI.Views
{
    public partial class KorisniciWindow : Window
    {
        private GlavniMediator mediator;
        private KorisniciMediator korisniciMediator;
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();
        KorisnikServis korisnikServis = new KorisnikServis();

        private string SelektovanaLokacija;
        private string SelektovanTipClanstva;
        private string SelektovanStatusaNaloga;

        List<Lokacija> Lokacije;
        List<TipClanstva> TipoviClanstva;
        List<string> StatusiNaloga;
        List<Korisnik> Korisnici;

        public KorisniciWindow(GlavniMediator mediator, KorisniciMediator mediator1, string imeLanca)
        {
            this.mediator = mediator;
            this.korisniciMediator = mediator1;
            facade = CoworkingFasada.DajInstancu();
            InitializeComponent();
            NazivLanca.Text = imeLanca;
        }

        public void Show()
        {
            Pretraga.Visibility = Visibility.Collapsed;
            Izmena.Visibility = Visibility.Collapsed;
            RefreshPretragaMeni();
            RefreshTable();

            base.Show();
        }

        //Osvezava sva polja sa izborom
        public void RefreshPretragaMeni()
        {
            //Praznjenje liste
            SelKorisnici_LokCBox.Items.Clear();
            SelKorisnici_TipClCBox.Items.Clear();
            SelKorisnici_StatusCBox.Items.Clear();

            //Popunjavanje liste
            Lokacije = facade.Lokacije.DajSve();
            SelKorisnici_LokCBox.Items.Add("Lokacija");
            SelektovanaLokacija = "Lokacija";
            foreach(Lokacija x in Lokacije)
                SelKorisnici_LokCBox.Items.Add(x.Ime);

            TipoviClanstva = facade.TipoviClanstva.DajSve();
            SelKorisnici_TipClCBox.Items.Add("TipClasntva");
            SelektovanTipClanstva = "TipClasntva";
            foreach (TipClanstva x in TipoviClanstva)
                SelKorisnici_TipClCBox.Items.Add(x.Ime);

            StatusiNaloga = korisnikServis.dajStatuseNaloga();
            SelKorisnici_StatusCBox.Items.Add("StatusNaloga");
            SelektovanStatusaNaloga = "StatusNaloga";
            foreach(string x in StatusiNaloga)
                SelKorisnici_StatusCBox.Items.Add(x);

            //Postavljanje trenutnih vrednistu na nultu
            SelKorisnici_LokCBox.SelectedIndex = 0;
            SelKorisnici_TipClCBox.SelectedIndex = 0;
            SelKorisnici_StatusCBox.SelectedIndex = 0;

        }

        //Osvezava prikazanu tabelu
        public void RefreshTable()
        {
            if (SelektovanaLokacija == "Lokacija")
                SelektovanaLokacija = null;

            if (SelektovanTipClanstva == "TipClasntva")
                SelektovanTipClanstva = null;

            if (SelektovanStatusaNaloga == "StatusNaloga")
                SelektovanStatusaNaloga = null;

            Korisnici = korisnikServis.dajKorisnike(SelektovanaLokacija, SelektovanTipClanstva, SelektovanStatusaNaloga);

            TabelaKorisnika.ItemsSource = null;
            TabelaKorisnika.ItemsSource = Korisnici;
        }

        //Promena podataka (dodaj, izmeni ili obrisi)
        public bool Update(int op)
        {
            string Ime = TextBoxIme.Text;
            string Prezime = TextBoxPrezime.Text;
            string Email = TextBoxEmail.Text;
            string BrojTelefona = TextBoxBrojTelefona.Text;
            string TipClanstva = TextBoxTipClanstva.Text;
            string DatumPocetkaClanstva = TextBoxDatumPocetkaClanstva.Text;
            string DatumIstekaClanstva = TextBoxDatumIstekaClanstva.Text;
            string StatusNaloga = TextBoxStatusNaloga.Text;
            if(string.IsNullOrWhiteSpace(Ime))
                return false;
            if (string.IsNullOrWhiteSpace(Prezime))
                return false;
            if (string.IsNullOrWhiteSpace(Email))
                Email = null;
            if (string.IsNullOrWhiteSpace(BrojTelefona))
                BrojTelefona = null;
            if (string.IsNullOrWhiteSpace(TipClanstva))
                TipClanstva = null;
            if (string.IsNullOrWhiteSpace(DatumPocetkaClanstva))
                DatumPocetkaClanstva = null;
            if (string.IsNullOrWhiteSpace(DatumIstekaClanstva))
                DatumIstekaClanstva = null;
            if (string.IsNullOrWhiteSpace(StatusNaloga))
                StatusNaloga = null;
            if (op == 0)
            {
                if (Email == null)
                    return false;
                if (BrojTelefona == null)
                    return false;
                if (TipClanstva == null)
                    return false;
                if (DatumPocetkaClanstva == null)
                    return false;
                if (DatumIstekaClanstva == null)
                    return false;
                if (StatusNaloga == null)
                    return false;

                //return korisnikServis.dodajKorisnika(Ime, Prezime, Email, BrojTelefona, TipClanstva, DatumPocetkaClanstva, DatumIstekaClanstva, StatusNaloga);
            }
            else if (op == 1)
            {
                //return korisnikServis.izmeniKorisnika(Ime, Prezime, Email, BrojTelefona, TipClanstva, DatumPocetkaClanstva, DatumIstekaClanstva, StatusNaloga);
            }
            else
            {
                return korisnikServis.obrisiKorisnika(Ime, Prezime);
            }

            //treuntni return zbog nedostatka funkcionalnosti servisa
            return false;
        }

        private void SelKorisnici_LokCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            SelektovanaLokacija = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
        }
        private void SelKorisnici_TipClCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            SelektovanTipClanstva = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
        }
        private void SelKorisnici_StatusCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string SelektovanStatusaNaloga = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
        }

        //Navigacione funkcije
        private void Pretraga_Click(object sender, RoutedEventArgs e)
        {
            korisniciMediator.Notify(this, "Meni_Pretraga");
        }

        private void Izmena_Click(object sender, RoutedEventArgs e)
        {
            korisniciMediator.Notify(this, "Meni_Izmena");
        }

        //Funkcije koja vrsi zeljene akcije
        private void Pretrazi_Click(object sender, RoutedEventArgs e)
        {
            korisniciMediator.Notify(this, "Pretrazi");
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            korisniciMediator.Notify(this, "Dodaj");
        }

        private void Izmeni_Click(object sender, RoutedEventArgs e)
        {
            korisniciMediator.Notify(this, "Izmeni");
        }

        private void Obrisi_Click(object sender, RoutedEventArgs e)
        {
            korisniciMediator.Notify(this, "Obrisi");
        }

        //Funkcija za povratak na Glavni Meni
        private void GlavniMeni_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_GlavniMeni");
        }

        private void KorisniciWindow_Closed(object sender, EventArgs e)
        {
            // This will shut down the entire application, closing all open windows.
            Application.Current.Shutdown();
        }
    }
}
