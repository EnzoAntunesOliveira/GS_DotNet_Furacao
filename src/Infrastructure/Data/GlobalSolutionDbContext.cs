using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data
{
    public class GlobalSolutionDbContext : DbContext
    {
        public GlobalSolutionDbContext(DbContextOptions<GlobalSolutionDbContext> options)
            : base(options) { }

        public DbSet<SafeHouse> SafeHouses { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Adm> Adms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<SafeHouse>().ToTable("SAFEHOUSES");
            modelBuilder.Entity<Usuario>().ToTable("USUARIOS");
            modelBuilder.Entity<Adm>().ToTable("ADMS");

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("UX_USUARIOS_EMAIL");

            modelBuilder.Entity<Adm>()
                .HasIndex(a => a.Email)
                .IsUnique()
                .HasDatabaseName("UX_ADMS_EMAIL");
        }
    }
}