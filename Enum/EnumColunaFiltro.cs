using System.ComponentModel.DataAnnotations;

namespace SistemaDeOcorrencias.Enum
{
    public enum EnumColunaFiltro
    {
        [Display(Name = "Ocorrência")]
        Ocorrencia,
        [Display(Name = "Tipo de Ocorrência")]
        TipoOcorrencia,
        [Display(Name = "Ocorreu em / Agendado para")]
        OcorreuEm,
        [Display(Name = "Transportador")]
        TransportadorDescricao,
        [Display(Name = "Transportador CNPJ/CPF")]
        TransportadorCnpj,
        [Display(Name = "Solução")]
        SolucaoEm
    }


}
