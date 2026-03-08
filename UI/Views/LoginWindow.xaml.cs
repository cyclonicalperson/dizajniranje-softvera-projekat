using System.Windows;
using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.UI.Mediator;

namespace CoWorkingManager.UI.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            KorisnikServisProxy proxy = new KorisnikServisProxy(new KorisnikServis());
            if (proxy.autentifikacija(UsernameBox.Text, PasswordBox.Password))
            {
                var glavniMediator = new GlavniMediator();
                var korisniciMediator = new KorisniciMediator();
                var lokacijeMediator = new LokacijeMediator(); // Novi mediator
                var tipoviClanstvaMediator = new TipoviClanstvaMediator(); // Novi mediator
                var resursiMediator = new ResursiMediator(); // Novi mediator
                var rezervacijeMediator = new RezervacijeMediator(); // Novi mediator

                var glavniMeni = new GlavniWindow(glavniMediator);
                var korisnici = new KorisniciWindow(glavniMediator, korisniciMediator);
                var lokacije = new LokacijeWindow(glavniMediator, lokacijeMediator);
                var tipoviClanstva = new TipoviClanstvaWindow(glavniMediator, tipoviClanstvaMediator);
                var resursi = new ResursiWindow(glavniMediator, resursiMediator);
                var rezervacije = new RezervacijeWindow(glavniMediator, rezervacijeMediator);

                glavniMediator.SetGlavniWindow(glavniMeni);
                glavniMediator.SetKorisnici(korisnici);
                korisniciMediator.SetKorisnici(korisnici);
                glavniMediator.SetLokacije(lokacije);
                lokacijeMediator.SetLokacije(lokacije);
                glavniMediator.SetTipoviClanstva(tipoviClanstva);
                tipoviClanstvaMediator.SetTipoviClanstva(tipoviClanstva);
                glavniMediator.SetResursi(resursi);
                resursiMediator.SetResursi(resursi);
                glavniMediator.SetRezervacije(rezervacije);
                rezervacijeMediator.SetRezervacije(rezervacije);

                glavniMeni.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Pogrešni kredencijali.");
            }
        }
    }
}