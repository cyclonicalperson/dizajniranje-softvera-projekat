using CoWorkingManager.Modeli;
using System.Windows;
using System;
using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Podaci;

namespace CoWorkingManager.UI.Views
{
    public partial class RezervacijeDialog : Window
    {
        public Rezervacija RezervacijaData { get; private set; }
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();
        private static RezervacijaServis rezervacijaServis = new RezervacijaServis();
        private RezervacijaServisProxy rezervacijaServisProxy = new RezervacijaServisProxy(rezervacijaServis);
        private static KorisnikServis korisnikServis = new KorisnikServis();
        private KorisnikServisProxy korisnikServisProxy = new KorisnikServisProxy(korisnikServis);
        private static LokacijaServis lokacijaServis = new LokacijaServis();
        private static LokacijaServisProxy lokacijaServisProxy = new LokacijaServisProxy(lokacijaServis);
        private static ResursServis resursServis = new ResursServis();
        private ResursServisProxy resursServisProxy = new ResursServisProxy(resursServis);
        private bool isEditMode = false;

        public RezervacijeDialog(Rezervacija existing = null)
        {
            InitializeComponent();
            // Popuni combobox sa korisnicima, resursima, lokacijama iz fasade
            ComboKorisnik.ItemsSource = facade.Korisnici.DajSve(); // Pretpostavi binding na ID/Name
            ComboLokacija.ItemsSource = facade.Lokacije.DajSve();
            ComboResurs.ItemsSource = facade.Resursi.DajSve();
            if (existing != null)
            {
                isEditMode = true;
                // Popuni za izmenu
                /*ComboKorisnik.SelectedValue = existing.KorisnikID;
                ComboLokacija.SelectedValue = existing.LokacijaID; // Dodaj LokacijaID u model ako treba
                ComboResurs.SelectedValue = existing.ResursID;
                DatePocetak.SelectedDate = existing.DatumPocetka;
                DateZavrsetak.SelectedDate = existing.DatumZavrsetka;
                if (existing.ResursTip == "sala") // Pretpostavi polje u modelu
                {
                    TxtBrojUcesnika.Text = existing.BrojUcesnika.ToString(); // Dodaj polje u model
                }*/
                BtnOtkaziRezervaciju.Visibility = Visibility.Visible; // Pokaži za edit mod
                BtnSacuvaj.Content = "Izmeni"; // Promeni tekst button-a
                RezervacijaData = existing; // Za izmenu
            }
            // Za sale, prikaži broj učesnika
            ComboResurs.SelectionChanged += (s, e) => {
                if (ComboResurs.SelectedItem != null && ComboResurs.SelectedItem.ToString().Contains("sala"))
                    PanelBrojUcesnika.Visibility = Visibility.Visible;
                else
                    PanelBrojUcesnika.Visibility = Visibility.Collapsed;
            };
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            // Kreiraj Rezervacija objekat
            RezervacijaData = RezervacijaData ?? new Rezervacija();
            /*RezervacijaData.KorisnikID = ComboKorisnik.SelectedValue.ToString();
            RezervacijaData.ResursID = ComboResurs.SelectedValue.ToString();
            RezervacijaData.LokacijaID = ComboLokacija.SelectedValue.ToString(); // Dodaj u model ako treba
            RezervacijaData.DatumPocetka = DatePocetak.SelectedDate ?? DateTime.Now;
            RezervacijaData.DatumZavrsetka = DateZavrsetak.SelectedDate ?? DateTime.Now;
            // Dodaj broj ucesnika ako sala
            if (PanelBrojUcesnika.Visibility == Visibility.Visible)
            {
                if (int.TryParse(TxtBrojUcesnika.Text, out int broj))
                    RezervacijaData.BrojUcesnika = broj;
                else
                {
                    ValidationError.Text = "Nevalidan broj učesnika.";
                    ValidationError.Visibility = Visibility.Visible;
                    return;
                }
            }

            // Validacija poslovnih pravila
            if (!rezervacijaServis.ValidirajRezervaciju(RezervacijaData)) // Pozovi servis za proveru zauzetosti, sati, radnog vremena
            {
                ValidationError.Text = "Rezervacija nije validna: zauzeto, prekoračeni sati ili van radnog vremena.";
                ValidationError.Visibility = Visibility.Visible;
                return;
            }

            // Sačuvaj (kreiraj ili izmeni)
            rezervacijaServis.KreirajIliIzmeniRezervaciju(RezervacijaData);
            DialogResult = true;*/
        }

        private void Izmeni_Click(object sender, RoutedEventArgs e)
        { 
        }

        private void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            /*RezervacijaData.Status = "otkazana"; // Promeni status
            rezervacijaServis.KreirajIliIzmeniRezervaciju(RezervacijaData); // Sačuvaj promenu
            DialogResult = true;*/
        }

        private void Zatvori_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}