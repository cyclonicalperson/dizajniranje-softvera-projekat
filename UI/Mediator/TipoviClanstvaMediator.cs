using CoWorkingManager.Mediator;
using CoWorkingManager.UI.Views;
using System.Windows;

namespace CoWorkingManager.UI.Mediator
{
    public class TipoviClanstvaMediator : IMediator
    {
        private TipoviClanstvaWindow tipoviClanstva;

        public void SetTipoviClanstva(TipoviClanstvaWindow w) => tipoviClanstva = w;

        public void Notify(Window sender, string eventCode)
        {
            switch (eventCode)
            {
                case "Meni_Pretraga":
                    tipoviClanstva.Pretraga.Visibility = Visibility.Visible;
                    tipoviClanstva.Izmena.Visibility = Visibility.Collapsed;
                    tipoviClanstva.Uspesno.Visibility = Visibility.Collapsed;
                    tipoviClanstva.Neuspesno.Visibility = Visibility.Collapsed;
                    tipoviClanstva.RefreshTable();
                    break;

                case "Meni_Izmena":
                    tipoviClanstva.Pretraga.Visibility = Visibility.Collapsed;
                    tipoviClanstva.Izmena.Visibility = Visibility.Visible;
                    tipoviClanstva.Uspesno.Visibility = Visibility.Collapsed;
                    tipoviClanstva.Neuspesno.Visibility = Visibility.Collapsed;
                    break;

                case "Dodaj":
                    tipoviClanstva.Uspesno.Visibility = Visibility.Collapsed;
                    tipoviClanstva.Neuspesno.Visibility = Visibility.Collapsed;
                    if (tipoviClanstva.Update(0))
                        tipoviClanstva.Uspesno.Visibility = Visibility.Visible;
                    else
                        tipoviClanstva.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Izmeni":
                    tipoviClanstva.Uspesno.Visibility = Visibility.Collapsed;
                    tipoviClanstva.Neuspesno.Visibility = Visibility.Collapsed;
                    if (tipoviClanstva.Update(1))
                        tipoviClanstva.Uspesno.Visibility = Visibility.Visible;
                    else
                        tipoviClanstva.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Obrisi":
                    tipoviClanstva.Uspesno.Visibility = Visibility.Collapsed;
                    tipoviClanstva.Neuspesno.Visibility = Visibility.Collapsed;
                    if (tipoviClanstva.Update(2))
                        tipoviClanstva.Uspesno.Visibility = Visibility.Visible;
                    else
                        tipoviClanstva.Neuspesno.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}