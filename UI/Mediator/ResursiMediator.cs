using CoWorkingManager.Mediator;
using CoWorkingManager.UI.Views;
using System.Windows;

namespace CoWorkingManager.UI.Mediator
{
    public class ResursiMediator : IMediator
    {
        private ResursiWindow resursi;

        public void SetResursi(ResursiWindow w) => resursi = w;

        public void Notify(Window sender, string eventCode)
        {
            switch (eventCode)
            {
                case "Meni_Pretraga":
                    resursi.Pretraga.Visibility = Visibility.Visible;
                    resursi.Izmena.Visibility = Visibility.Collapsed;
                    resursi.Uspesno.Visibility = Visibility.Collapsed;
                    resursi.Neuspesno.Visibility = Visibility.Collapsed;
                    resursi.RefreshPretragaMeni();
                    resursi.RefreshTable();
                    break;

                case "Meni_Izmena":
                    resursi.Pretraga.Visibility = Visibility.Collapsed;
                    resursi.Izmena.Visibility = Visibility.Visible;
                    resursi.Uspesno.Visibility = Visibility.Collapsed;
                    resursi.Neuspesno.Visibility = Visibility.Collapsed;
                    break;

                case "Dodaj":
                    resursi.Uspesno.Visibility = Visibility.Collapsed;
                    resursi.Neuspesno.Visibility = Visibility.Collapsed;
                    if (resursi.Update(0))
                        resursi.Uspesno.Visibility = Visibility.Visible;
                    else
                        resursi.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Izmeni":
                    resursi.Uspesno.Visibility = Visibility.Collapsed;
                    resursi.Neuspesno.Visibility = Visibility.Collapsed;
                    if (resursi.Update(1))
                        resursi.Uspesno.Visibility = Visibility.Visible;
                    else
                        resursi.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Obrisi":
                    resursi.Uspesno.Visibility = Visibility.Collapsed;
                    resursi.Neuspesno.Visibility = Visibility.Collapsed;
                    if (resursi.Update(2))
                        resursi.Uspesno.Visibility = Visibility.Visible;
                    else
                        resursi.Neuspesno.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}