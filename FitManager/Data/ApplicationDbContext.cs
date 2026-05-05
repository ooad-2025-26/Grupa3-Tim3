using FitManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitManager.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Korisnik> Korisnici => Set<Korisnik>();
        public DbSet<QRKod> QRKodovi => Set<QRKod>();
        public DbSet<TipClanarine> TipoviClanarine => Set<TipClanarine>();
        public DbSet<Clanarina> Clanarine => Set<Clanarina>();
        public DbSet<GrupniTrening> GrupniTreninzi => Set<GrupniTrening>();
        public DbSet<Rezervacija> Rezervacije => Set<Rezervacija>();
        public DbSet<Dolazak> Dolasci => Set<Dolazak>();
        public DbSet<PlanTreninga> PlanoviTreninga => Set<PlanTreninga>();
        public DbSet<Izvjestaj> Izvjestaji => Set<Izvjestaj>();
        public DbSet<EmailObavjestenje> EmailObavjestenja => Set<EmailObavjestenje>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Korisnik>(entity =>
            {
                entity.ToTable("Korisnik");
                entity.HasKey(korisnik => korisnik.Id);
                entity.Property(korisnik => korisnik.Email)
                    .HasMaxLength(256)
                    .IsRequired();
                entity.Property(korisnik => korisnik.KorisnickoIme)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(korisnik => korisnik.Uloga)
                    .HasConversion<int>();
                entity.Property(korisnik => korisnik.Ime)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(korisnik => korisnik.Prezime)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(korisnik => korisnik.Telefon)
                    .HasMaxLength(30);
                entity.HasOne(korisnik => korisnik.QRKod)
                    .WithOne(qrKod => qrKod.Clan)
                    .HasForeignKey<QRKod>(qrKod => qrKod.ClanId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(korisnik => korisnik.Clanarine)
                    .WithOne(clanarina => clanarina.Clan)
                    .HasForeignKey(clanarina => clanarina.ClanId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(korisnik => korisnik.Rezervacije)
                    .WithOne(rezervacija => rezervacija.Clan)
                    .HasForeignKey(rezervacija => rezervacija.ClanId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(korisnik => korisnik.Dolasci)
                    .WithOne(dolazak => dolazak.Clan)
                    .HasForeignKey(dolazak => dolazak.ClanId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(korisnik => korisnik.PlanoviTreninga)
                    .WithOne(plan => plan.Clan)
                    .HasForeignKey(plan => plan.ClanId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(korisnik => korisnik.GrupniTreninzi)
                    .WithOne(trening => trening.Trener)
                    .HasForeignKey(trening => trening.TrenerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(korisnik => korisnik.Izvjestaji)
                    .WithOne(izvjestaj => izvjestaj.Administrator)
                    .HasForeignKey(izvjestaj => izvjestaj.AdministratorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<QRKod>(entity =>
            {
                entity.ToTable("QRKod");
                entity.HasKey(qrKod => qrKod.Id);
                entity.Property(qrKod => qrKod.Kod)
                    .HasMaxLength(128)
                    .IsRequired();
                entity.HasIndex(qrKod => qrKod.Kod)
                    .IsUnique();
            });

            modelBuilder.Entity<TipClanarine>(entity =>
            {
                entity.ToTable("TipClanarine");
                entity.HasKey(tip => tip.Id);
                entity.Property(tip => tip.Naziv)
                    .HasConversion<int>();
                entity.Property(tip => tip.Cijena)
                    .HasColumnType("decimal(18,2)");
                entity.HasIndex(tip => tip.Naziv)
                    .IsUnique();
            });

            modelBuilder.Entity<Clanarina>(entity =>
            {
                entity.ToTable("Clanarina");
                entity.HasKey(clanarina => clanarina.Id);
                entity.Property(clanarina => clanarina.Cijena)
                    .HasColumnType("decimal(18,2)");
                entity.Property(clanarina => clanarina.Status)
                    .HasConversion<int>();
                entity.HasOne(clanarina => clanarina.TipClanarine)
                    .WithMany(tip => tip.Clanarine)
                    .HasForeignKey(clanarina => clanarina.TipClanarineId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GrupniTrening>(entity =>
            {
                entity.ToTable("GrupniTrening");
                entity.HasKey(trening => trening.Id);
                entity.Property(trening => trening.Naziv)
                    .HasMaxLength(150)
                    .IsRequired();
                entity.Property(trening => trening.Opis)
                    .HasMaxLength(1000);
                entity.Property(trening => trening.TipTreninga)
                    .HasConversion<int>();
            });

            modelBuilder.Entity<Rezervacija>(entity =>
            {
                entity.ToTable("Rezervacija");
                entity.HasKey(rezervacija => new { rezervacija.ClanId, rezervacija.GrupniTreningId });
                entity.Property(rezervacija => rezervacija.Status)
                    .HasConversion<int>();
                entity.HasOne(rezervacija => rezervacija.GrupniTrening)
                    .WithMany(trening => trening.Rezervacije)
                    .HasForeignKey(rezervacija => rezervacija.GrupniTreningId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Dolazak>(entity =>
            {
                entity.ToTable("Dolazak");
                entity.HasKey(dolazak => dolazak.Id);
            });

            modelBuilder.Entity<PlanTreninga>(entity =>
            {
                entity.ToTable("PlanTreninga");
                entity.HasKey(plan => plan.Id);
                entity.Property(plan => plan.FitnessCilj)
                    .HasConversion<int>();
                entity.Property(plan => plan.Bmi)
                    .HasColumnType("decimal(18,2)");
                entity.Property(plan => plan.BmiKategorija)
                    .HasConversion<int>();
                entity.Property(plan => plan.Intenzitet)
                    .HasConversion<int>();
                entity.Property(plan => plan.SedmicniPlan)
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<Izvjestaj>(entity =>
            {
                entity.ToTable("Izvjestaj");
                entity.HasKey(izvjestaj => izvjestaj.Id);
                entity.Property(izvjestaj => izvjestaj.TipIzvjestaja)
                    .HasConversion<int>();
                entity.Property(izvjestaj => izvjestaj.Sadrzaj)
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<EmailObavjestenje>(entity =>
            {
                entity.ToTable("EmailObavjestenje");
                entity.HasKey(obavjestenje => obavjestenje.Id);
                entity.Property(obavjestenje => obavjestenje.Status)
                    .HasConversion<int>();
                entity.Property(obavjestenje => obavjestenje.Sadrzaj)
                    .HasMaxLength(4000);
                entity.HasOne(obavjestenje => obavjestenje.Clanarina)
                    .WithMany(clanarina => clanarina.EmailObavjestenja)
                    .HasForeignKey(obavjestenje => obavjestenje.ClanarinaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
