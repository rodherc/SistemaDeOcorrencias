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
        public DbSet<OcorrenciaModalView> OcorrenciaModalViews { get; set; }

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

            modelBuilder.Entity<OcorrenciaModalView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("view_ocorrencia_modal");
            });

            base.OnModelCreating(modelBuilder);
        }
    }

    public class OcorrenciaModalView
    {
        public long Numero { get; set; }
        public DateTime OcorreuEm { get; set; }
        public TimeSpan Horario { get; set; }
        public DateTime? Lancamento { get; set; }
        public string TipoCodigo { get; set; }
        public string TipoDescricao { get; set; }
        public string TransportadoraCNPJ { get; set; }
        public string TransportadoraDescricao { get; set; }
        public string Documento { get; set; }
        public string Pedido { get; set; }
        public string Localizacao { get; set; }
        public string ResponsavelSolucao { get; set; }
        public string Interessado { get; set; }
        public string ResponsavelInteressado { get; set; }
        public string OrigemInformacao { get; set; }
        public string Contato { get; set; }
        public string RgCpf { get; set; }
        public string Responsavel { get; set; }
        public string DepartamentoPadrao { get; set; }
        public string UsuarioResponsavel { get; set; }
        public string CidadeDestino { get; set; }
        public string EstadoDestino { get; set; }
        public string Observacoes { get; set; }
    }
}
