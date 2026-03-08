using System.IO;
using System.Windows;
using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Podaci;
using CoWorkingManager.UI.Views;

namespace CoWorkingManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!File.Exists("config.txt"))
            {
                MessageBox.Show("config.txt nije pronađen!");
                Shutdown();
                return;
            }

            var lines = File.ReadAllLines("config.txt");

            if (lines.Length < 2)
            {
                MessageBox.Show("config.txt nema konekcioni string!");
                Shutdown();
                return;
            }

            string connectionString = lines[1];
            CoworkingFasada.Inicijalizuj(connectionString);

            CoworkingFasada fasada = CoworkingFasada.DajInstancu();
            // Ne obelezavamo gotove rezervacije jer necemo vise imati aktivne rezervacije
            //fasada.Rezervacije.ObeleziZavrseneRezervacije();

            // Pokrecemo automatski dnevni izvoz izveštaja
            // Izvoz se dešava odmah pri pokretanju, pa zatim svakih 24h
            IzvestajServis.Instanca.Pokreni(IzvestajServis.PeriodIzvoza.SvakiDan);

            LoginWindow login = new LoginWindow();
            login.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Zaustavljamo tajmer pri zatvaranju da se proces ugasi cisto
            IzvestajServis.Instanca.Zaustavi();
            base.OnExit(e);
        }
    }
}