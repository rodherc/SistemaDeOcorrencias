using System.ComponentModel.DataAnnotations;

namespace SistemaDeOcorrencias.Models
{
    public class Ocorrencia
    {
        public long Id { get; set; }

        [Display(Name = "Tipo de Ocorrência")]
        public long Id_Tipo { get; set; }
        public Tipo Tipo { get; set; }

        [Display(Name = "Transportador")]
        public long Id_Transportador { get; set; }
        public Transportador Transportador { get; set; }

        [Display(Name = "Ocorreu em")]
        public DateTime Ocorreu_Em { get; set; }

        [Display(Name = "Solução em")]
        public DateTime? Solucao_Em { get; set; }
    }
}
