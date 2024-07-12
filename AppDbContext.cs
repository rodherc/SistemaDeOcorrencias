using Microsoft.EntityFrameworkCore;
using SistemaDeOcorrencias.Models;

namespace SistemaDeOcorrencias
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Ocorrencia> Ocorrencia { get; set; }
        public DbSet<Transportador> Transportador { get; set; }
        public DbSet<Tipo> Tipo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ocorrencia>()
                .HasOne(o => o.Tipo)
                .WithMany()
                .HasForeignKey(o => o.Id_Tipo);

            modelBuilder.Entity<Ocorrencia>()
                .HasOne(o => o.Transportador)
                .WithMany()
                .HasForeignKey(o => o.Id_Transportador);

            base.OnModelCreating(modelBuilder);
        }
    }
}
