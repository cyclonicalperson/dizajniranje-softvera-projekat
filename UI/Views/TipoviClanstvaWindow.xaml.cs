using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using System.Windows;

namespace CoWorkingManager.UI.Views
{
    public partial class TipoviClanstvaWindow : Window
    {
        private GlavniMediator mediator;
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();

        public TipoviClanstvaWindow(GlavniMediator mediator, string imeLanca)
        {
            this.mediator = mediator;
            InitializeComponent();
            NazivLanca.Text = imeLanca;

            this.mediator = mediator;

            RefreshTable();
        }

        public void RefreshTable()
        {
            List<TipClanstva> lista = facade.TipoviClanstva.DajSve();

            ClanstvaGrid.ItemsSource = null;
            ClanstvaGrid.ItemsSource = lista;
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            TipClanstvaServis servis = new TipClanstvaServis();
            //servis.dodajTipClanstva();

            RefreshTable();
            mediator.Notify(this, "CLANSTVA_CHANGED");
        }

        private void TipoviClanstvaWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
