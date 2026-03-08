using CoWorkingManager.Logika.Servisi;
using CoWorkingManager.Mediator;
using CoWorkingManager.Modeli;
using CoWorkingManager.Podaci;
using CoWorkingManager.UI.Mediator;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System;

namespace CoWorkingManager.UI.Views
{
    public partial class RezervacijeWindow : Window
    {
        private GlavniMediator mediator;
        private RezervacijeMediator rezervacijeMediator;
        private CoworkingFasada facade = CoworkingFasada.DajInstancu();
        private RezervacijaServis rezervacijaServis = new RezervacijaServis(); // Pretpostavljamo servis

        private string SelektovaniKorisnik;
        private DateTime? SelektovaniDan;
        private string SelektovanaLokacija;

        List<Korisnik> Korisnici;
        List<Lokacija> Lokacije;
        List<Rezervacija> Rezervacije;

        public RezervacijeWindow(GlavniMediator mediator, RezervacijeMediator rezervacijeMediator)
        {
            this.mediator = mediator;
            this.rezervacijeMediator = rezervacijeMediator;
            InitializeComponent();
        }

        private void GlavniMeni_Click(object sender, RoutedEventArgs e)
        {
            mediator.Notify(this, "Otvori_GlavniMeni");
        }
    }
}