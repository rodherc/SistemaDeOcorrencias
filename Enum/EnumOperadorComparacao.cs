using System.ComponentModel.DataAnnotations;

namespace SistemaDeOcorrencias.Enum
{
    public enum EnumOperadorComparacao
    {
        [Display(Name = "Igual a")]
        IgualA = 0,
        [Display(Name = "Maior que")]
        MaiorQue = 1,
        [Display(Name = "Menor que")]
        MenorQue = 2
    }
}
