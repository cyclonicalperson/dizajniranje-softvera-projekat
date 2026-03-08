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
                    sender.Hide();
                    glavniMeni.Show();
                    break;

                case "Otvori_Korisnike":
                    sender.Hide();
                    korisnici.Show();
                    break;

                case "Otvori_Lokacije":
                    sender.Hide();
                    lokacije.Show();
                    break;

                case "Otvori_TipoveClanstva":
                    sender.Hide();
                    tipoviClanstva.Show();
                    break;

                case "Otvori_Resurse":
                    sender.Hide();
                    resursi.Show();
                    break;

                case "Otvori_ResurseSaLokacijom":
                    string? nazivLokacije = lokacije.IzabranaNazivLokacije;
                    if (nazivLokacije != null)
                    {
                        sender.Hide();
                        resursi.ShowSaLokacijom(nazivLokacije);
                    }
                    break;

                case "Otvori_Rezervacije":
                    sender.Hide();
                    rezervacije.Show();
                    break;
            }
        }
    }
}