using Microsoft.EntityFrameworkCore;
using CoWorkingManager.Modeli;

namespace CoWorkingManager.Podaci
{
    // Glavni EF DbContext za aplikaciju
    // Podrzava MSSQL i MySQL — tip baze se određuje automatski iz konekcionog stringa
    // Implementira Singleton obrazac: samo jedna instanca postoji tokom rada aplikacije
    public class KontekstBaze : DbContext
    {
        // ── Singleton implementacija ─────────────────────────────────────────

        private static KontekstBaze? instanca;
        private static readonly object katanac = new();

        private readonly string konekcioniString = string.Empty;
        private readonly TipBaze tipBaze;

        // Moguci tipovi baza koje aplikacija podržava
        public enum TipBaze { MSSQL, MySQL }

        // Privatni konstruktor — sprečava kreiranje instance spolja
        private KontekstBaze(string konekcioniString, TipBaze tipBaze)
        {
            this.konekcioniString = konekcioniString;
            this.tipBaze = tipBaze;
        }

        // Prazan konstruktor neophodan za EF Core design-time migracije
        public KontekstBaze() { }

        // Vraca jedinu instancu konteksta (thread-safe Singleton)
        // Tip baze se automatski detektuje iz konekcionog stringa
        public static KontekstBaze DajInstancu(string konekcioniString)
        {
            if (instanca == null)
            {
                lock (katanac)
                {
                    if (instanca == null)
                    {
                        var tip = DetektujTipBaze(konekcioniString);
                        instanca = new KontekstBaze(konekcioniString, tip);
                    }
                }
            }
            return instanca;
        }

        // Resetuje Singleton — za promenu konfiguracije ili testove
        public static void ResetInstance()
        {
            lock (katanac)
            {
                instanca?.Dispose();
                instanca = null;
            }
        }

        // ── DbSet-ovi (tabele) ────────────────────────────────────────────────

        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<TipClanstva> TipoviClanstava { get; set; }
        public DbSet<Lokacija> Lokacije { get; set; }
        public DbSet<Resurs> Resursi { get; set; }
        public DbSet<Rezervacija> Rezervacije { get; set; }
        public DbSet<Administrator> Administratori { get; set; }

        // ── Konfiguracija konekcije ───────────────────────────────────────────

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                if (tipBaze == TipBaze.MySQL)
                    builder.UseMySql(konekcioniString, ServerVersion.AutoDetect(konekcioniString));
                else
                    builder.UseSqlServer(konekcioniString);
            }
        }

        // ── Fluent API konfiguracija ──────────────────────────────────────────

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            KonfigurisatiTipClanstva(mb);
            KonfigurisatiLokaciju(mb);
            KonfigurisatiKorisnika(mb);
            KonfigurisatiResurs(mb);
            KonfigurisatiRezervaciju(mb);
            KonfigurisatiAdministratora(mb);
        }

        private static void KonfigurisatiTipClanstva(ModelBuilder mb)
        {
            mb.Entity<TipClanstva>(e =>
            {
                e.ToTable("TipoviClanstava");
                e.HasKey(t => t.Id);
                e.HasIndex(t => t.Ime).IsUnique();
                e.Property(t => t.Ime).IsRequired().HasMaxLength(50);
                e.Property(t => t.Cena).HasColumnType("decimal(10,2)");
                e.Property(t => t.Trajanje).IsRequired();
                e.Property(t => t.MaxSatiPoMesecu).IsRequired();
                e.Property(t => t.PristupSali).IsRequired();
                e.Property(t => t.BrojSatiUSaliMesecno).IsRequired(false);
            });
        }

        private static void KonfigurisatiLokaciju(ModelBuilder mb)
        {
            mb.Entity<Lokacija>(e =>
            {
                e.ToTable("Lokacije");
                e.HasKey(l => l.Id);
                e.Property(l => l.Ime).IsRequired().HasMaxLength(100);
                e.Property(l => l.Adresa).IsRequired().HasMaxLength(200);
                e.Property(l => l.Grad).IsRequired().HasMaxLength(50);
                e.Property(l => l.RadniSati).IsRequired().HasMaxLength(100);
                e.Property(l => l.MaxBrojKorisnika).IsRequired();
            });
        }

        private static void KonfigurisatiKorisnika(ModelBuilder mb)
        {
            mb.Entity<Korisnik>(e =>
            {
                e.ToTable("Korisnici");
                e.HasKey(k => k.Id);
                e.HasIndex(k => k.Email).IsUnique();
                e.Property(k => k.Ime).IsRequired().HasMaxLength(50);
                e.Property(k => k.Prezime).IsRequired().HasMaxLength(50);
                e.Property(k => k.Email).IsRequired().HasMaxLength(100);
                e.Property(k => k.Telefon).HasMaxLength(30);

                // DATE u bazi — EF Core mapira DateOnly na DATE kolonu (SQL Server 2019+/LocalDB)
                e.Property(k => k.DatumPocetkaClanstva).HasColumnType("date");
                e.Property(k => k.DatumKrajaClanstva).HasColumnType("date");

                // Enum se cuva kao string malim slovima: 'aktivan', 'pauziran', 'istekao'
                e.Property(k => k.StatusNaloga)
                 .HasConversion(
                     v => v.ToString().ToLower(),
                     v => Enum.Parse<StatusNaloga>(
                         char.ToUpper(v[0]) + v.Substring(1), true))
                 .HasMaxLength(20);

                // Relacija: Korisnik - TipClanstva (CASCADE zabranjen — ne briši tip ako ima korisnika)
                e.HasOne(k => k.TipClanstva)
                 .WithMany(t => t.Korisnici)
                 .HasForeignKey(k => k.TipClanstvaId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Indeksi iz schema.sql
                e.HasIndex(k => k.StatusNaloga).HasDatabaseName("IX_Korisnici_Status");
                e.HasIndex(k => k.TipClanstvaId).HasDatabaseName("IX_Korisnici_TipClanstva");
            });
        }

        private static void KonfigurisatiResurs(ModelBuilder mb)
        {
            mb.Entity<Resurs>(e =>
            {
                e.ToTable("Resursi");
                e.HasKey(r => r.Id);
                e.Property(r => r.Ime).IsRequired().HasMaxLength(100);
                e.Property(r => r.Opis).HasMaxLength(300);

                // TipResursa: enum - string sa vrednostima iz baze ('sto', 'sala', 'privatna_kancelarija')
                e.Property(r => r.TipResursa)
                 .HasConversion(
                     v => v == TipResursa.PrivatnaKancelarija ? "privatna_kancelarija"
                        : v.ToString().ToLower(),
                     v => v == "privatna_kancelarija" ? TipResursa.PrivatnaKancelarija
                        : Enum.Parse<TipResursa>(
                            char.ToUpper(v[0]) + v.Substring(1), true))
                 .HasMaxLength(30);

                // PodtipStola: 'hot_desk' | 'dedicated_desk' | NULL
                e.Property(r => r.PodtipStola)
                 .HasConversion(
                     v => v == null ? null
                        : v == PodtipStola.HotDesk ? "hot_desk" : "dedicated_desk",
                     v => v == null ? (PodtipStola?)null
                        : v == "hot_desk" ? PodtipStola.HotDesk : PodtipStola.DedicatedDesk)
                 .HasMaxLength(30);

                // Kolone za sale i kancelarije su nullable
                e.Property(r => r.Kapacitet).IsRequired(false);
                e.Property(r => r.ImaProjektor).IsRequired(false);
                e.Property(r => r.ImaTV).IsRequired(false);
                e.Property(r => r.ImaTablu).IsRequired(false);
                e.Property(r => r.ImaOnlineOpremu).IsRequired(false);

                // Relacija: Resurs - Lokacija
                e.HasOne(r => r.Lokacija)
                 .WithMany(l => l.Resursi)
                 .HasForeignKey(r => r.LokacijaId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Indeksi iz schema.sql
                e.HasIndex(r => r.LokacijaId).HasDatabaseName("IX_Resursi_Lokacija");
                e.HasIndex(r => r.TipResursa).HasDatabaseName("IX_Resursi_Tip");
            });
        }

        private static void KonfigurisatiRezervaciju(ModelBuilder mb)
        {
            mb.Entity<Rezervacija>(e =>
            {
                e.ToTable("Rezervacije");
                e.HasKey(r => r.Id);

                // DATETIME2 u bazi — DateTime u C# mapira direktno
                e.Property(r => r.PocetakVreme).HasColumnType("datetime2");
                e.Property(r => r.KrajVreme).HasColumnType("datetime2");

                // StatusRezervacije: 'aktivna' | 'zavrsena' | 'otkazana'
                e.Property(r => r.StatusRezervacije)
                 .HasConversion(
                     v => v.ToString().ToLower(),
                     v => Enum.Parse<StatusRezervacije>(
                         char.ToUpper(v[0]) + v.Substring(1), true))
                 .HasMaxLength(20);

                // Relacija: Rezervacija - Korisnik
                e.HasOne(r => r.Korisnik)
                 .WithMany(k => k.Rezervacije)
                 .HasForeignKey(r => r.KorisnikId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Relacija: Rezervacija - Resurs (Restrict — ne briše resurs ako ima rezervacija)
                e.HasOne(r => r.Resurs)
                 .WithMany(res => res.Rezervacije)
                 .HasForeignKey(r => r.ResursId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Indeksi iz schema.sql
                e.HasIndex(r => r.KorisnikId).HasDatabaseName("IX_Rezervacije_Korisnik");
            });
        }

        private static void KonfigurisatiAdministratora(ModelBuilder mb)
        {
            mb.Entity<Administrator>(e =>
            {
                e.ToTable("Administratori");
                e.HasKey(a => a.Id);
                e.HasIndex(a => a.KorisnickoIme).IsUnique();
                e.HasIndex(a => a.Email).IsUnique();
                e.Property(a => a.KorisnickoIme).IsRequired().HasMaxLength(50);
                e.Property(a => a.Lozinka).IsRequired().HasMaxLength(50);
                e.Property(a => a.Ime).IsRequired().HasMaxLength(50);
                e.Property(a => a.Prezime).IsRequired().HasMaxLength(50);
                e.Property(a => a.Email).IsRequired().HasMaxLength(100);

                // DATE u bazi — koristimo DateOnly
                e.Property(a => a.DatumKreiranja)
                 .HasColumnType("date")
                 .HasDefaultValueSql("GETDATE()");
            });
        }

        // ── Detekcija tipa baze ───────────────────────────────────────────────

        private static TipBaze DetektujTipBaze(string ks)
        {
            var lower = ks.ToLower();
            if (lower.Contains("port=3306") || lower.Contains("mysql") ||
                lower.Contains("allowuservariables") || lower.Contains("charset=utf8"))
                return TipBaze.MySQL;
            return TipBaze.MSSQL;
        }
    }
}