using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using CoWorkingManager.UI.Mediator;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System;

namespace CoWorkingManager.UI.Views
{
    public partial class KorisniciWindow : Window
    {
        private GlavniMediator mediator;
        private KorisniciMediator korisniciMediator;
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();
        private static KorisnikServis korisnikServis = new KorisnikServis();
        private KorisnikServisProxy korisnikServisProxy = new KorisnikServisProxy(korisnikServis);

        private string SelektovanaLokacija;
        private string SelektovanTipClanstva;
        private string SelektovanStatusaNaloga;

        List<Lokacija> Lokacije;
        List<TipClanstva> TipoviClanstva;
        List<string> StatusiNaloga;
        List<Korisnik> Korisnici;

        public KorisniciWindow(GlavniMediator mediator, KorisniciMediator korisniciMediator)
        {
            this.mediator = mediator;
            this.korisniciMediator = korisniciMediator;
            facade = CoworkingFasada.DajInstancu();
            InitializeComponent();
            //Dodati naziv lanca
            //Lanac.Text = 
        }

        public void Show()
        {
            Pretraga.Visibility = Visibility.Collapsed;
            Izmena.Visibility = Visibility.Collapsed;
            RefreshPretragaMeni();
            RefreshTable();

            base.Show();
        }

        public void RefreshPretragaMeni()
        {
            SelKorisnici_LokCBox.Items.Clear();
            SelKorisnici_TipClCBox.Items.Clear();
            SelKorisnici_StatusCBox.Items.Clear();

            Lokacije = facade.Lokacije.DajSve();
            SelKorisnici_LokCBox.Items.Add("Lokacija");
            SelektovanaLokacija = "Lokacija";
            foreach (Lokacija x in Lokacije)
                SelKorisnici_LokCBox.Items.Add(x.Ime);

            TipoviClanstva = facade.TipoviClanstva.DajSve();
            SelKorisnici_TipClCBox.Items.Add("TipClanstva");
            SelektovanTipClanstva = "TipClanstva";
            foreach (TipClanstva x in TipoviClanstva)
                SelKorisnici_TipClCBox.Items.Add(x.Ime);

            StatusiNaloga = korisnikServis.dajStatuseNaloga();
            SelKorisnici_StatusCBox.Items.Add("StatusNaloga");
            SelektovanStatusaNaloga = "StatusNaloga";
            foreach (string x in StatusiNaloga)
                SelKorisnici_StatusCBox.Items.Add(x);

            SelKorisnici_LokCBox.SelectedIndex = 0;
            SelKorisnici_TipClCBox.SelectedIndex = 0;
            SelKorisnici_StatusCBox.SelectedIndex = 0;
        }

        public void RefreshTable()
        {
            if (SelektovanaLokacija == "Lokacija")
                SelektovanaLokacija = null;

            if (SelektovanTipClanstva == "TipClanstva")
                SelektovanTipClanstva = null;

            if (SelektovanStatusaNaloga == "StatusNaloga")
                SelektovanStatusaNaloga = null;

            Korisnici = korisnikServis.dajKorisnike(SelektovanaLokacija, SelektovanTipClanstva, SelektovanStatusaNaloga);

            TabelaKorisnika.ItemsSource = null;
            TabelaKorisnika.ItemsSource = Korisnici;
        }

        public bool Update(int op)
        {
            string? Ime = TextBoxIme.Text;
            string? Prezime = TextBoxPrezime.Text;
            string? Email = TextBoxEmail.Text;
            string? BrojTelefona = TextBoxBrojTelefona.Text;
            string? TipClanstva = TextBoxTipClanstva.Text;
            DateTime? PocetakClanstva = TextBoxDatumPocetkaClanstva.SelectedDate;
            DateOnly? DatumPocetkaClanstva = null;
            if (PocetakClanstva != null)
            {
                DatumPocetkaClanstva = DateOnly.FromDateTime(TextBoxDatumPocetkaClanstva.SelectedDate.Value);
            }
            DateTime? IstekClanstva = TextBoxDatumPocetkaClanstva.SelectedDate;
            DateOnly? DatumIstekaClanstva = null;
            if (IstekClanstva != null)
            {
                DatumIstekaClanstva = DateOnly.FromDateTime(TextBoxDatumIstekaClanstva.SelectedDate.Value);
            }
            string? StatusNaloga = TextBoxStatusNaloga.Text;

            if (string.IsNullOrWhiteSpace(Ime) || string.IsNullOrWhiteSpace(Prezime))
                return false;

            if (op == 0) // Dodaj
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(BrojTelefona) || string.IsNullOrWhiteSpace(TipClanstva) ||
                    DatumPocetkaClanstva == null || DatumIstekaClanstva == null || string.IsNullOrWhiteSpace(StatusNaloga))
                    return false;
                else

                    return korisnikServisProxy.dodajKorisnika(Ime, Prezime, Email, BrojTelefona, TipClanstva, (DateOnly)DatumPocetkaClanstva, (DateOnly)DatumIstekaClanstva, StatusNaloga);
            }
            else if (op == 1) // Izmeni
            {
                return korisnikServisProxy.izmeniKorisnika(Ime, Prezime, Email, BrojTelefona, TipClanstva, DatumPocetkaClanstva, DatumIstekaClanstva, StatusNaloga);
            }
            else // Obrisi
            {
                return korisnikServisProxy.obrisiKorisnika(Ime, Prezime);
            }
        }

        private void SelKorisnici_LokCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelektovanaLokacija = comboBox.SelectedItem.ToString();
        }

        private void SelKorisnici_TipClCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelektovanTipClanstva = comboBox.SelectedItem.ToString();
        }

        private void SelKorisnici_StatusCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelektovanStatusaNaloga = comboBox.SelectedItem.ToString();
        }

        private void Pretraga_Click(object sender, RoutedEventArgs e)
        {
            korisniciMediator.Notify(this, "Meni_Pretraga");
        }

        private void Izmena_Click(object sender, RoutedEventArgs e)
        {
            korisniciMediator.Notify(this, "Meni_Izmena");
        }

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

        private void GlavniMeni_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_GlavniMeni");
        }
    }
}