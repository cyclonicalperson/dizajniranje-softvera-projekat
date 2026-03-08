using CoWorkingManager.Mediator;
using CoWorkingManager.UI.Views;
using System.Windows;

namespace CoWorkingManager.UI.Mediator
{
    public class RezervacijeMediator : IMediator
    {
        private RezervacijeWindow rezervacije;

        public void SetRezervacije(RezervacijeWindow w) => rezervacije = w;

        public void Notify(Window sender, string eventCode)
        {
            switch (eventCode)
            {
                case "Meni_PretragaKorisnik":
                    rezervacije.PretragaKorisnik.Visibility = Visibility.Visible;
                    rezervacije.PretragaDanLokacija.Visibility = Visibility.Collapsed;
                    rezervacije.RefreshPretragaKorisnikMeni();
                    rezervacije.RefreshTableKorisnik();
                    break;

                case "Meni_PretragaDanLokacija":
                    rezervacije.PretragaKorisnik.Visibility = Visibility.Collapsed;
                    rezervacije.PretragaDanLokacija.Visibility = Visibility.Visible;
                    rezervacije.RefreshPretragaDanLokacijaMeni();
                    break;

                case "RefreshAfterChange":
                    rezervacije.RefreshTableKorisnik();
                    rezervacije.RefreshTableDanLokacija();
                    break;
            }
        }
    }
}