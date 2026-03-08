using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.UI.Views;
using System.Windows;

namespace CoWorkingManager.UI.Mediator
{
    public class RezervacijeDialogMediator : IMediator
    {
        private RezervacijeDialog? rezervacija;
        private RezervacijeMediator? rezervacijeMediator;

        public void SetKorisnici(RezervacijeDialog w) => rezervacija = w;

        public void SetRezervacijeMediator(RezervacijeMediator m) => rezervacijeMediator = m;

        public void Notify(Window sender, string eventCode)
        {
            if (rezervacija == null) return;

            switch (eventCode)
            {
                case "Dodaj":
                    rezervacija.Uspesno.Visibility = Visibility.Collapsed;
                    rezervacija.Neuspesno.Visibility = Visibility.Collapsed;
                    if (rezervacija.Update(0))
                    {
                        rezervacija.Uspesno.Visibility = Visibility.Visible;
                        rezervacijeMediator?.Notify(sender, "RefreshAfterChange");
                    }
                    else
                        rezervacija.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Izmeni":
                    rezervacija.Uspesno.Visibility = Visibility.Collapsed;
                    rezervacija.Neuspesno.Visibility = Visibility.Collapsed;
                    if (rezervacija.Update(1))
                    {
                        rezervacija.Uspesno.Visibility = Visibility.Visible;
                        rezervacijeMediator?.Notify(sender, "RefreshAfterChange");
                    }
                    else
                        rezervacija.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Otkazi":
                    rezervacija.Uspesno.Visibility = Visibility.Collapsed;
                    rezervacija.Neuspesno.Visibility = Visibility.Collapsed;
                    if (rezervacija.Update(2))
                    {
                        rezervacija.Uspesno.Visibility = Visibility.Visible;
                        rezervacijeMediator?.Notify(sender, "RefreshAfterChange");
                    }
                    else
                        rezervacija.Neuspesno.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}