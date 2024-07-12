using Microsoft.EntityFrameworkCore;
using SistemaDeOcorrencias.Enum;
using SistemaDeOcorrencias.Models;

namespace SistemaDeOcorrencias.Services
{
    public class OcorrenciaService
    {
        private readonly AppDbContext _context;
        public OcorrenciaService(AppDbContext context) {
            _context = context;
        }

        public async Task<(List<Ocorrencia>,int totalRegistros)> CarregarOcorrencias(int pagina, FiltroDto dto)
        {
            const int ItensPorPagina = 10; // Número de itens por página

            IQueryable<Ocorrencia> query = _context.Ocorrencia
                .Include(o => o.Tipo)
                .Include(o => o.Transportador);

            // Verifica se o DTO de filtro não está vazio e aplica os filtros se necessário
            if (dto != null && dto.Filtros != null && dto.Filtros.Any())
            {
                foreach (var filtro in dto.Filtros)
                {
                    string coluna = filtro.Coluna;
                    string busca = filtro.Busca;
                    int operador = filtro.Operador;

                    switch (coluna)
                    {
                        case "Ocorrencia":
                            long id;
                            if (long.TryParse(busca, out id))
                            {
                                switch (operador)
                                {
                                    case (int)EnumOperadorComparacao.IgualA: 
                                        query = query.Where(o => o.Id == id);
                                        break;
                                    case (int)EnumOperadorComparacao.MaiorQue:
                                        query = query.Where(o => o.Id > id);
                                        break;
                                    case (int)EnumOperadorComparacao.MenorQue:
                                        query = query.Where(o => o.Id < id);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        case "SolucaoEm":
                            DateTime data;
                            if (DateTime.TryParse(busca, out data))
                            {
                                switch (operador)
                                {
                                    case (int)EnumOperadorComparacao.IgualA:
                                        query = query.Where(o => o.Solucao_Em == data);
                                        break;
                                    case (int)EnumOperadorComparacao.MaiorQue:
                                        query = query.Where(o => o.Solucao_Em > data);
                                        break;
                                    case (int)EnumOperadorComparacao.MenorQue:
                                        query = query.Where(o => o.Solucao_Em < data);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case "OcorreuEm":
                            DateTime dataBusca;
                            if (DateTime.TryParse(busca, out dataBusca))
                            {
                                switch (operador)
                                {
                                    case (int)EnumOperadorComparacao.IgualA:
                                        query = query.Where(o => o.Ocorreu_Em == dataBusca);
                                        break;
                                    case (int)EnumOperadorComparacao.MaiorQue:
                                        query = query.Where(o => o.Ocorreu_Em > dataBusca);
                                        break;
                                    case (int)EnumOperadorComparacao.MenorQue:
                                        query = query.Where(o => o.Ocorreu_Em < dataBusca);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        case "TransportadorDescricao":
                            query = query.Where(o => o.Transportador.Descricao == busca);
                            break;

                        case "TransportadorCnpj":
                            query = query.Where(o => o.Transportador.CNPJ == busca);
                            break;

                        case "TipoOcorrencia":
                            query = query.Where(o => o.Tipo.Descricao == busca);
                            break;

                        default:
                            break;
                    }
                }
            }

            // Ordenação e paginação
            var ocorrencias = await query
                .OrderByDescending(o => o.Ocorreu_Em)
                .Skip((pagina - 1) * ItensPorPagina)
                .Take(ItensPorPagina)
                .ToListAsync();

            int totalRegistros = await query.CountAsync();

            return (ocorrencias, totalRegistros);
        }

        public async Task<OcorrenciaModalView> Detalhes(int idOcorrencia)
        {
            try
            {
                var view = await _context.OcorrenciaModalViews
                .FirstOrDefaultAsync(o => o.Numero == idOcorrencia);

                return view;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public async Task<int> Count()
        {
            try
            {
                return await _context.Ocorrencia.CountAsync();
            }
            catch(Exception exeption)
            {
                return 0;
            }
        }
    }  
}
