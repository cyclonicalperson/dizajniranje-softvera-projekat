using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.UI.Views;
using System.Windows;

namespace CoWorkingManager.UI.Mediator
{
    public class KorisniciMediator : IMediator
    {
        private KorisniciWindow korisnici;

        public void SetKorisnici(KorisniciWindow w) => korisnici = w;

        public void Notify(Window sender, string eventCode)
        {
            switch (eventCode)
            {
                case "Meni_Pretraga":
                    korisnici.Pretraga.Visibility = Visibility.Visible;
                    korisnici.Izmena.Visibility = Visibility.Collapsed;
                    korisnici.Uspesno.Visibility = Visibility.Collapsed;
                    korisnici.Neuspesno.Visibility = Visibility.Collapsed;

                    korisnici.RefreshPretragaMeni();
                    korisnici.RefreshTable();
                    break;

                case "Meni_Izmena":
                    korisnici.Pretraga.Visibility = Visibility.Collapsed;
                    korisnici.Izmena.Visibility = Visibility.Visible;
                    korisnici.Uspesno.Visibility = Visibility.Collapsed;
                    korisnici.Neuspesno.Visibility = Visibility.Collapsed;
                    break;

                case "Pretrazi":
                    korisnici.RefreshTable();
                    break;

                case "Dodaj":
                    korisnici.Uspesno.Visibility = Visibility.Collapsed;
                    korisnici.Neuspesno.Visibility = Visibility.Collapsed;
                    if (korisnici.Update(0))
                        korisnici.Uspesno.Visibility = Visibility.Visible;
                    else
                        korisnici.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Izmeni":
                    korisnici.Uspesno.Visibility = Visibility.Collapsed;
                    korisnici.Neuspesno.Visibility = Visibility.Collapsed;
                    if (korisnici.Update(1))
                        korisnici.Uspesno.Visibility = Visibility.Visible;
                    else
                        korisnici.Neuspesno.Visibility = Visibility.Visible;
                    break;

                case "Obrisi":
                    korisnici.Uspesno.Visibility = Visibility.Collapsed;
                    korisnici.Neuspesno.Visibility = Visibility.Collapsed;
                    if (korisnici.Update(2))
                        korisnici.Uspesno.Visibility = Visibility.Visible;
                    else
                        korisnici.Neuspesno.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
