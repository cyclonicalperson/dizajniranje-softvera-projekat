using System;
using CoWorkingManager.Podaci;
using CoWorkingManager.Modeli;
using BCrypt.Net;

namespace CoWorkingManager.Logika.Servisi
{
    public class AdministratorServis
    {
        private readonly CoworkingFasada _fasada;

        public AdministratorServis()
        {
            _fasada = CoworkingFasada.DajInstancu();
        }
        public Administrator Login(string korisnickoIme, string lozinka)
        {
            Administrator admin = _fasada.Administratori.DajPoKorisnickomImenu(korisnickoIme);

            if (admin != null && BCrypt.Net.BCrypt.Verify(lozinka, admin.HashLozinke))
            {
                return admin;
            }

            return null;
        }
    }
}