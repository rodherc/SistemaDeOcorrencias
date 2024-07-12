using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeOcorrencias.Models;

namespace SistemaDeOcorrencias.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Index(int pagina = 1)
    {
        const int ItensPorPagina = 10; // Número de itens por página

        int totalRegistros = await _context.Ocorrencia.CountAsync();
        int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)ItensPorPagina);

        ViewBag.PaginaAtual = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        ViewBag.totalRegistros = totalRegistros;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CarregarOcorrencias(int pagina, [FromBody] FiltroDto dto)
    {
        try
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
                        case "Id":
                            long id;
                            if (long.TryParse(busca, out id))
                            {
                                query = query.Where(o => o.Id == id);
                            }
                            break;

                        case "Solucao_Em":
                            DateTime data;
                            if (DateTime.TryParse(busca, out data))
                            {
                                switch (operador)
                                {
                                    case 0: // Igual a (ou padrão)
                                        query = query.Where(o => o.Solucao_Em == data);
                                        break;
                                    case 2: // Maior que
                                        query = query.Where(o => o.Solucao_Em > data);
                                        break;
                                    case 3: // Menor que
                                        query = query.Where(o => o.Solucao_Em < data);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case "Ocorreu_Em":
                            DateTime dataBusca;
                            if (DateTime.TryParse(busca, out dataBusca))
                            {
                                switch (operador)
                                {
                                    case 0:
                                        query = query.Where(o => o.Ocorreu_Em == dataBusca);
                                        break;
                                    case 2:
                                        query = query.Where(o => o.Ocorreu_Em > dataBusca);
                                        break;
                                    case 3:
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

            // Formatação das ocorrências para envio
            var ocorrenciasFormatadas = ocorrencias.Select(o => new
            {
                o.Id,
                TipoOcorrencia = o.Tipo.Descricao,
                OcorreuEm = o.Ocorreu_Em.ToString("dd/MM/yyyy HH:mm"),
                TransportadorDescricao = o.Transportador.Descricao,
                TransportadorCnpj = o.Transportador.CNPJ,
                SolucaoEm = o.Solucao_Em.HasValue ? o.Solucao_Em.Value.ToString("dd/MM/yyyy HH:mm") : "-",
                JaSolucionada = o.Solucao_Em.HasValue ? "Sim" : "Não",
                DiasEmAberto = (DateTime.Now - o.Ocorreu_Em).Days 
            });

            int totalRegistros = await query.CountAsync();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)ItensPorPagina);

            return Json(new { ocorrencias = ocorrenciasFormatadas, totalPaginas, paginaAtual = pagina });
        }
        catch (Exception ex)
        {
            return BadRequest("Erro ao carregar ocorrências: " + ex.Message);
        }
    }
}

