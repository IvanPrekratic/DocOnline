using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zavrsni.Model;

namespace Zavrsni.DAL
{
    public class DataManagerDbContext : IdentityDbContext<AppUser>
    {

        public DbSet<Doktor> Doktori { get; set; }
        public DbSet<Pacijent> Pacijenti { get; set; }
        public DbSet<Pregled> Pregledi { get; set; }
        public DbSet<Specijalizacija> Specijalizacije { get; set; }

        public DataManagerDbContext(DbContextOptions<DataManagerDbContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 1, Naziv = "Opća praksa" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 2, Naziv = "Kardiologija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 3, Naziv = "Dermatologija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 4, Naziv = "Ginekologija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 5, Naziv = "Interna medicina" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 6, Naziv = "Neurologija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 7, Naziv = "Oftalmologija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 8, Naziv = "Ortopedija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 9, Naziv = "Otorinolaringologija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 10, Naziv = "Pedijatrija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 11, Naziv = "Psihijatrija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 12, Naziv = "Radiologija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 13, Naziv = "Stomatologija" });
            modelBuilder.Entity<Specijalizacija>().HasData(new Specijalizacija { SpecijalizacijaID = 14, Naziv = "Urologija" });
        }

    }
}
