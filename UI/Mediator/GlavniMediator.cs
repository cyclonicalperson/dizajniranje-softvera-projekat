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

        public void SetGlavniWindow(GlavniWindow w) => glavniMeni = w;
        public void SetKorisnici(KorisniciWindow w) => korisnici = w;
        public void SetLokacije(LokacijeWindow w) => lokacije = w;
        public void SetTipoviClanstva(TipoviClanstvaWindow w) => tipoviClanstva = w;
        public void SetResursi(ResursiWindow w) => resursi = w;
        public void SetRezervacije(RezervacijeWindow w) => rezervacije = w;


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