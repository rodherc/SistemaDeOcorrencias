using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaDeOcorrencias.Models
{
    public class FiltroDto
    {
        public List<FiltroItem> Filtros { get; set; }
    }

    public class FiltroItem
    {
        public string Coluna { get; set; }
        public int Operador { get; set; }
        public string Busca { get; set; }
    }
}
