using CoWorkingManager.Modeli;
using System.Windows;
using System;

namespace CoWorkingManager.UI.Views
{
    public partial class RezervacijeDialog : Window
    {
        public Rezervacija RezervacijaData { get; private set; } // Podaci za rezervaciju

        public RezervacijeDialog(Rezervacija existing = null)
        {
            InitializeComponent();
            // Popuni combobox sa korisnicima, resursima, lokacijama iz fasade
            // Ako existing, popuni polja
            if (existing != null)
            {
                // Popuni za izmenu
            }
            // Za sale, prikaži broj učesnika
            ComboResurs.SelectionChanged += (s, e) => {
                if (ComboResurs.SelectedItem != null && ComboResurs.SelectedItem.ToString().Contains("sala"))
                    PanelBrojUcesnika.Visibility = Visibility.Visible;
                else
                    PanelBrojUcesnika.Visibility = Visibility.Collapsed;
            };
        }

        private void Sacuvaj_Click(object sender, RoutedEventArgs e)
        {
            // Kreiraj Rezervacija objekat
            /*RezervacijaData = new Rezervacija
            {
                KorisnikID = ComboKorisnik.SelectedValue.ToString(),
                ResursID = ComboResurs.SelectedValue.ToString(),
                DatumPocetka = DatePocetak.SelectedDate ?? DateTime.Now,
                DatumZavrsetka = DateZavrsetak.SelectedDate ?? DateTime.Now,
                // Dodaj broj ucesnika ako sala
            };*/
            DialogResult = true;
        }

        private void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}