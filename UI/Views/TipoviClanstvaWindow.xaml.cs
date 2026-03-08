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
    public partial class TipoviClanstvaWindow : Window
    {
        private GlavniMediator mediator;
        private TipoviClanstvaMediator tipoviClanstvaMediator;
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();
        private static TipClanstvaServis tipClanstvaServis = new TipClanstvaServis();
        private TipClanstvaServisProxy tipClanstvaServisProxy = new TipClanstvaServisProxy(tipClanstvaServis);

        List<TipClanstva> TipoviClanstva;

        public TipoviClanstvaWindow(GlavniMediator mediator, TipoviClanstvaMediator tipoviClanstvaMediator)
        {
            this.mediator = mediator;
            this.tipoviClanstvaMediator = tipoviClanstvaMediator;
            InitializeComponent();
            string[] configLines = File.ReadAllLines("config.txt");
            Lanac.Text = configLines[0];
        }

        public void Show()
        {
            Pretraga.Visibility = Visibility.Collapsed;
            Izmena.Visibility = Visibility.Collapsed;
            base.Show();
        }

        public void RefreshTable()
        {
            TipoviClanstva = facade.TipoviClanstva.DajSve();
            TabelaTipovaClanstva.ItemsSource = null;
            TabelaTipovaClanstva.ItemsSource = TipoviClanstva;
        }

        public bool Update(int op)
        {
            string? NazivPaketa = TextBoxNazivPaketa.Text;
            decimal? Cena = decimal.TryParse(TextBoxCena.Text, out decimal value1) ? value1 : null;
            int? Trajanje = int.TryParse(TextBoxTrajanje.Text, out int value2) ? value2 : null;
            int? MaksimalanBrojSati = int.TryParse(TextBoxMaksimalanBrojSati.Text, out int value3) ? value3 : null;
            bool DozvolaZaSale = CheckBoxDozvolaZaSale.IsChecked ?? false;
            int? BrojSatiZaSale = int.TryParse(TextBoxBrojSatiZaSale.Text, out int value4) ? value4 : null;

            if (string.IsNullOrWhiteSpace(NazivPaketa) || Cena == 0 || Trajanje == 0)
                return false;

            if (op == 0) // Dodaj
            {
                if (string.IsNullOrWhiteSpace(NazivPaketa) || Cena == null || Trajanje == null || MaksimalanBrojSati == null)
                    return false;
                return tipClanstvaServisProxy.dodajTipClanstva((string)NazivPaketa, (decimal)Cena, (int)Trajanje, (int)MaksimalanBrojSati, DozvolaZaSale, BrojSatiZaSale);
            }
            else if (op == 1) // Izmeni
            {
                return false; //tipClanstvaServisProxy.izmeniTipClanstva(NazivPaketa, Cena, Trajanje, MaksimalanBrojSati, DozvolaZaSale, BrojSatiZaSale);
            }
            else // Obrisi
            {
                return false; //tipClanstvaServisProxy.obrisiTipClanstva(NazivPaketa);
            }
        }

        private void Pretraga_Click(object sender, RoutedEventArgs e)
        {
            tipoviClanstvaMediator.Notify(this, "Meni_Pretraga");
        }

        private void Izmena_Click(object sender, RoutedEventArgs e)
        {
            tipoviClanstvaMediator.Notify(this, "Meni_Izmena");
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            tipoviClanstvaMediator.Notify(this, "Dodaj");
        }

        private void Izmeni_Click(object sender, RoutedEventArgs e)
        {
            tipoviClanstvaMediator.Notify(this, "Izmeni");
        }

        private void Obrisi_Click(object sender, RoutedEventArgs e)
        {
            tipoviClanstvaMediator.Notify(this, "Obrisi");
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