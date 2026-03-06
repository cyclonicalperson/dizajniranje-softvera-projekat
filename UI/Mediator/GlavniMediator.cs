using CoWorkingManager.Modeli;
using CoWorkingManager.UI.Views;
using System.Windows;
using System.Windows.Controls;

namespace CoWorkingManager.Mediator
{
    public class GlavniMediator : IMediator
    {
        private GlavniWindow glavniMeni;
        private KorisniciWindow korisnici;
        private LokacijeWindow lokacije;
        private TipoviClanstvaWindow tipoviClanstva;
        private ResursiWindow resursi;
        private RezervacijeWindow rezervacije;

        public void SetMain(GlavniWindow w) => glavniMeni = w;
        public void SetUsers(KorisniciWindow w) => korisnici = w;
        public void SetLocations(LokacijeWindow w) => lokacije = w;
        public void SetMemberships(TipoviClanstvaWindow w) => tipoviClanstva = w;
        public void SetResources(ResursiWindow w) => resursi = w;
        public void SetReservations(RezervacijeWindow w) => rezervacije = w;


        public void Notify(Window sender, string eventCode)
        {
            switch (eventCode)
            {
                case "Otvori_GlavniMeni":
                    sender.Close();
                    glavniMeni.Show();
                    break;

                case "Otvori_Korisnike":
                    sender.Close();
                    korisnici.Show();
                    break;

                case "Otvori_Lokacije":
                    sender.Close();
                    lokacije.Show();
                    break;

                case "Otvori_TipoveClanstva":
                    sender.Close();
                    tipoviClanstva.Show();
                    break;

                case "Otvori_Resurse":
                    sender.Close();
                    resursi.Show();
                    break;

                case "Otvori_Rezervacije":
                    sender.Close();
                    rezervacije.Show();
                    break;
            }
        }
    }
}