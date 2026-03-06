using System;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Logika.Servisi
{
    public class AdminSession
    {
        private static AdminSession instance;
        public Administrator Admin { get; private set; }

        private AdminSession() { }

        public static AdminSession Instance
        {
            get
            {
                if (instance == null)
                    instance = new AdminSession();
                return instance;
            }
        }

        public void Login(Administrator admin)
        {
            Admin = admin;
        }

        public void Logout()
        {
            Admin = null;
        }
    }
}
