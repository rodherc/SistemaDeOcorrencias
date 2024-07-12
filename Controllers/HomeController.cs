using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaDeOcorrencias.Enum;
using SistemaDeOcorrencias.Models;
using SistemaDeOcorrencias.Services;

namespace SistemaDeOcorrencias.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    private readonly OcorrenciaService _ocorrenciaService;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
        _ocorrenciaService = new OcorrenciaService(_context);
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

        int totalRegistros = await _ocorrenciaService.Count();
        int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)ItensPorPagina);

        ViewBag.PaginaAtual = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        ViewBag.ColunaFiltroOptions = EnumColunaFiltro.GetValues(typeof(EnumColunaFiltro))
                                          .Cast<EnumColunaFiltro>()
                                          .Select(v => new SelectListItem
                                          {
                                              Text = v.GetDisplayName(),
                                              Value = v.ToString()
                                          });

        ViewBag.CondicaoNumericaOptions = EnumOperadorComparacao.GetValues(typeof(EnumOperadorComparacao))
                                              .Cast<EnumOperadorComparacao>()
                                              .Select(v => new SelectListItem
                                              {
                                                  Text = EnumExtensions.GetDisplayName(v),
                                                  Value = ((int)v).ToString()
                                              });
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CarregarOcorrencias(int pagina, [FromBody] FiltroDto dto)
    {
        try
        {
            const int ItensPorPagina = 10; // Número de itens por página

            (var ocorrencias, int totalRegistros) = await _ocorrenciaService.CarregarOcorrencias(pagina, dto);

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

            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)ItensPorPagina);

            return Json(new { ocorrencias = ocorrenciasFormatadas, totalPaginas, paginaAtual = pagina , totalRegistros = totalRegistros});
        }
        catch (Exception ex)
        {
            return BadRequest("Erro ao carregar ocorrências: " + ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalhes(int idOcorrencia)
    {
        var ocorrencia = await _ocorrenciaService.Detalhes(idOcorrencia);

        if (ocorrencia is null)
        {
            return NotFound();
        }

        return Json(ocorrencia);
    }
}

