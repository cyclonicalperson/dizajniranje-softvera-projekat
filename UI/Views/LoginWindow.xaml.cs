using System.Windows;
using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.UI.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class LoginWindow : Window
    {
        private GlavniMediator mediator;
        private string imeLanca;

        public LoginWindow(GlavniMediator mediator, string imeLanca)
        {
            this.mediator = mediator;
            InitializeComponent();
            this.imeLanca = imeLanca;
            NazivLanca.Text = imeLanca;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            KorisnikServisProxy proxy = new KorisnikServisProxy(new KorisnikServis());
            if (proxy.autentifikacija(UsernameBox.Text, PasswordBox.Password))
            {
                var mediator = new GlavniMediator();
                var mediatorKorisnici = new KorisniciMediator();

                var glavniMeni = new GlavniWindow(mediator, imeLanca);
                var korisnici = new KorisniciWindow(mediator, mediatorKorisnici, imeLanca);
                var lokacije = new LokacijeWindow(mediator, imeLanca);
                var tipoviClanstva = new TipoviClanstvaWindow(mediator, imeLanca);
                var resursi = new ResursiWindow(mediator, imeLanca);
                var rezervacije = new RezervacijeWindow(mediator, imeLanca);

                mediator.SetGlavniWindow(glavniMeni);
                mediator.SetKorisnici(korisnici);
                mediatorKorisnici.SetKorisnici(korisnici);
                mediator.SetLokacije(lokacije);
                mediator.SetTipoviClanstva(tipoviClanstva);
                mediator.SetResursi(resursi);
                mediator.SetRezervacije(rezervacije);

                glavniMeni.Show();
                this.Close();
            }
        }
    }
}