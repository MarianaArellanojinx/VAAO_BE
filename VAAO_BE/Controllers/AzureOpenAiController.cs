using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;
using VAAO_BE.Services;

namespace VAAO_BE.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AzureOpenAiController : ControllerBase
{
    private readonly IAzureOpenAiService _azureOpenAiService;
    private readonly IVentasRepository _ventasRepository;

    public AzureOpenAiController(IAzureOpenAiService azureOpenAiService, IVentasRepository ventasRepository)
    {
        _azureOpenAiService = azureOpenAiService;
        _ventasRepository = ventasRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Chat(OpenAiChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new
            {
                data = false,
                message = "El campo prompt es obligatorio.",
                status = false
            });
        }

        try
        {
            var result = await _azureOpenAiService.CreateChatCompletionAsync(request, cancellationToken);
            return Ok(new
            {
                data = result,
                message = string.Empty,
                status = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                data = false,
                message = ex.Message,
                status = false
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ResumenVentas(VentasAiSummaryRequest request, CancellationToken cancellationToken)
    {
        var ventas = await _ventasRepository.GetVentas();

        if (request.FechaInicio.HasValue && request.FechaFin.HasValue && request.FechaInicio.Value.Date > request.FechaFin.Value.Date)
        {
            return BadRequest(new
            {
                data = false,
                message = "La fechaInicio no puede ser mayor que la fechaFin.",
                status = false
            });
        }

        if (request.FechaInicio.HasValue || request.FechaFin.HasValue)
        {
            ventas = ventas
                .Where(v =>
                    v.FechaEntrega.HasValue &&
                    (!request.FechaInicio.HasValue || v.FechaEntrega.Value.Date >= request.FechaInicio.Value.Date) &&
                    (!request.FechaFin.HasValue || v.FechaEntrega.Value.Date <= request.FechaFin.Value.Date))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Cliente))
        {
            ventas = ventas
                .Where(v => v.NombreCliente.Contains(request.Cliente.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (ventas.Count == 0)
        {
            return Ok(new
            {
                data = false,
                message = "No se encontraron ventas para los filtros enviados.",
                status = false
            });
        }

        var ventasPorCliente = ventas
            .GroupBy(v => v.NombreCliente)
            .Select(group => new
            {
                Cliente = group.Key,
                Ventas = group.Count(),
                Bolsas = group.Sum(x => x.BolsasCompradas),
                Total = group.Sum(x => x.TotalPagar)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        var ventasPorDia = ventas
            .Where(v => v.FechaEntrega.HasValue)
            .GroupBy(v => v.FechaEntrega!.Value.Date)
            .Select(group => new
            {
                Fecha = group.Key.ToString("yyyy-MM-dd"),
                Ventas = group.Count(),
                Bolsas = group.Sum(x => x.BolsasCompradas),
                Total = group.Sum(x => x.TotalPagar)
            })
            .OrderBy(x => x.Fecha)
            .ToList();

        var detalleVentas = ventas
            .OrderByDescending(v => v.FechaEntrega)
            .Take(80)
            .Select(v => new
            {
                v.IdVenta,
                v.IdPedido,
                v.NombreCliente,
                v.NombreRepartidor,
                v.BolsasCompradas,
                v.TotalPagar,
                FechaEntrega = v.FechaEntrega?.ToString("yyyy-MM-dd HH:mm:ss"),
                v.MetodoPago
            })
            .ToList();

        var pregunta = string.IsNullOrWhiteSpace(request.Pregunta)
            ? "Dame un resumen ejecutivo de las ventas y destaca clientes importantes, tendencias y observaciones utiles."
            : request.Pregunta.Trim();

        var analysisRequest = new OpenAiChatRequest
        {
            SystemPrompt =
                "Eres un analista de ventas. Responde solo con base en los datos proporcionados. " +
                "Si falta informacion, dilo claramente. Responde en espanol de forma breve, clara y accionable.",
            Prompt =
                $"Analiza estas ventas y responde la siguiente pregunta: {pregunta}\n\n" +
                $"Filtros aplicados:\n" +
                $"- Fecha inicio: {(request.FechaInicio.HasValue ? request.FechaInicio.Value.ToString("yyyy-MM-dd") : "sin filtro")}\n" +
                $"- Fecha fin: {(request.FechaFin.HasValue ? request.FechaFin.Value.ToString("yyyy-MM-dd") : "sin filtro")}\n" +
                $"- Cliente: {(string.IsNullOrWhiteSpace(request.Cliente) ? "sin filtro" : request.Cliente.Trim())}\n\n" +
                $"Resumen por cliente:\n{System.Text.Json.JsonSerializer.Serialize(ventasPorCliente)}\n\n" +
                $"Resumen por dia:\n{System.Text.Json.JsonSerializer.Serialize(ventasPorDia)}\n\n" +
                $"Detalle de ventas:\n{System.Text.Json.JsonSerializer.Serialize(detalleVentas)}",
            MaxTokens = 500
        };

        try
        {
            var result = await _azureOpenAiService.CreateChatCompletionAsync(analysisRequest, cancellationToken);

            return Ok(new
            {
                data = new
                {
                    answer = result.Answer,
                    model = result.Model,
                    usage = result.Usage,
                    totalVentas = ventas.Count,
                    totalImporte = ventas.Sum(v => v.TotalPagar),
                    totalBolsas = ventas.Sum(v => v.BolsasCompradas),
                    topClientes = ventasPorCliente.Take(5)
                },
                message = string.Empty,
                status = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                data = false,
                message = ex.Message,
                status = false
            });
        }
    }
}
