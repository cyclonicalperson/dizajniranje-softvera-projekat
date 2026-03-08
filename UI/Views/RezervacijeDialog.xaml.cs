using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using CoWorkingManager.UI.Mediator;
using System;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;

namespace CoWorkingManager.UI.Views
{
    public partial class RezervacijeDialog : Window
    {
        private RezervacijeDialogMediator mediator;

        private RezervacijaServisProxy rezervacijaServisProxy = new RezervacijaServisProxy(new RezervacijaServis());
        private KorisnikServisProxy korisnikServisProxy = new KorisnikServisProxy(new KorisnikServis());
        private LokacijaServisProxy lokacijaServisProxy = new LokacijaServisProxy(new LokacijaServis());
        private ResursServisProxy resursServisProxy = new ResursServisProxy(new ResursServis());

        private List<Korisnik>? Korisnici;
        private List<Lokacija>? Lokacije;
        private List<Resurs>? Resursi;

        private Korisnik? SelectedKorisnik;
        private Lokacija? SelectedLokacija;
        private Resurs? SelectedResurs;

        public RezervacijeDialog(RezervacijeDialogMediator mediator)
        {
            InitializeComponent();
            this.mediator = mediator;
            mediator.SetKorisnici(this);

            SelectedKorisnik = null;
            SelectedLokacija = null;
            SelectedResurs = null;
            Korisnici = korisnikServisProxy.dajSve();
            ComboKorisnik.ItemsSource = Korisnici;
            Lokacije = lokacijaServisProxy.dajSve();
            ComboLokacija.ItemsSource = Lokacije;
            Resursi = null;
        }

        public void RefreshResursi()
        {
            Resursi = resursServisProxy.dajResursePoLokacijiSortiranoPoTipu(SelectedLokacija.Ime);
            ComboResurs.ItemsSource = Resursi;
        }

        public bool Update(int op)
        {
            if (SelectedKorisnik == null || SelectedResurs == null)
                return false;

            DateTime? Pocetak = DateZavrsetak.SelectedDate;
            DateOnly? DatumPocetak = null;
            if (Pocetak != null)
            {
                DatumPocetak = DateOnly.FromDateTime((DateTime)Pocetak);
            }
            DateTime? Kraj = DateZavrsetak.SelectedDate;
            DateOnly? DatumKraj = null;
            if (Kraj != null)
            {
                DatumKraj = DateOnly.FromDateTime((DateTime)Kraj);
            }
            string? vremePocetak = TimePocetak.Text;
            string? vremeKraj = TimePocetak.Text;

            if (op == 0)
            {
                rezervacijaServisProxy.kreirajRezervaciju(SelectedKorisnik, SelectedResurs, DatumPocetak, vremePocetak, DatumKraj, vremeKraj);
            }
            else if(op == 1)
            {
                rezervacijaServisProxy.izmeniRezervaciju(SelectedKorisnik, SelectedResurs, DatumPocetak, vremePocetak, DatumKraj, vremeKraj);
            }   
            else
            {
                rezervacijaServisProxy.otkaziRezervaciju(SelectedKorisnik, SelectedResurs);
            }

            return false;
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Dodaj");
        }

        private void Izmeni_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Izmeni");
        }

        private void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otkazi");
        }

        private void ComboKorisnik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelectedKorisnik = (Korisnik)comboBox.SelectedItem;
        }

        private void ComboLokacija_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelectedLokacija = (Lokacija)comboBox.SelectedItem;
            RefreshResursi();
        }

        private void ComboResurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
                SelectedResurs = (Resurs)comboBox.SelectedItem;
        }
    }
}