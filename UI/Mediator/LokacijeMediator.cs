using CoWorkingManager.Mediator;
using CoWorkingManager.UI.Views;
using System.Windows;

namespace CoWorkingManager.UI.Mediator
{
    public class LokacijeMediator : IMediator
    {
        private LokacijeWindow lokacije;

        public void SetLokacije(LokacijeWindow w) => lokacije = w;

        public void Notify(Window sender, string eventCode)
        {
            switch (eventCode)
            {
                case "Meni_Pretraga":
                    lokacije.Pretraga.Visibility = Visibility.Visible;
                    lokacije.Izmena.Visibility = Visibility.Collapsed;
                    lokacije.Uspesno.Visibility = Visibility.Collapsed;
                    lokacije.Neuspesno.Visibility = Visibility.Collapsed;
                    lokacije.RefreshTable();
                    break;

                case "Meni_Izmena":
                    lokacije.Pretraga.Visibility = Visibility.Collapsed;
                    lokacije.Izmena.Visibility = Visibility.Visible;
                    lokacije.Uspesno.Visibility = Visibility.Collapsed;
                    lokacije.Neuspesno.Visibility = Visibility.Collapsed;
                    break;

                case "Dodaj":
                    lokacije.Uspesno.Visibility = Visibility.Collapsed;
                    lokacije.Neuspesno.Visibility = Visibility.Collapsed;
                    if (lokacije.Update(0))
                        lokacije.Uspesno.Visibility = Visibility.Visible;
                    else
                        lokacije.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Izmeni":
                    lokacije.Uspesno.Visibility = Visibility.Collapsed;
                    lokacije.Neuspesno.Visibility = Visibility.Collapsed;
                    if (lokacije.Update(1))
                        lokacije.Uspesno.Visibility = Visibility.Visible;
                    else
                        lokacije.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Obrisi":
                    lokacije.Uspesno.Visibility = Visibility.Collapsed;
                    lokacije.Neuspesno.Visibility = Visibility.Collapsed;
                    if (lokacije.Update(2))
                        lokacije.Uspesno.Visibility = Visibility.Visible;
                    else
                        lokacije.Neuspesno.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}