using System.IO;
using System.Windows;
using CoWorkingManager.Mediator;
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
            String imeLanca = fasada.Config;

            GlavniMediator mediator = new GlavniMediator();

            LoginWindow login = new LoginWindow(mediator, imeLanca);
            login.Show();
        }
    }
}