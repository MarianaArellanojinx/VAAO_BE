using System.Text;
using System.Text.Json;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Services;

public class SalesAiAssistantService : ISalesAiAssistantService
{
    private readonly IAzureOpenAiService _azureOpenAiService;
    private readonly IAiConversationMemory _conversationMemory;
    private readonly IVentasRepository _ventasRepository;

    public SalesAiAssistantService(
        IAzureOpenAiService azureOpenAiService,
        IAiConversationMemory conversationMemory,
        IVentasRepository ventasRepository)
    {
        _azureOpenAiService = azureOpenAiService;
        _conversationMemory = conversationMemory;
        _ventasRepository = ventasRepository;
    }

    public async Task<AppAiChatResponse> ChatAsync(AppAiChatRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new ArgumentException("El mensaje es obligatorio.", nameof(request));
        }

        var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
            ? Guid.NewGuid().ToString("N")
            : request.ConversationId.Trim();

        var ventas = await _ventasRepository.GetVentas();
        var distinctClients = ventas
            .Select(v => v.NombreCliente)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(name => name.Length)
            .ToList();

        var detectedClient = string.IsNullOrWhiteSpace(request.Cliente)
            ? DetectClientFromMessage(request.Message, distinctClients)
            : request.Cliente.Trim();

        var (fechaInicio, fechaFin) = ResolveDateRange(request);
        var filteredSales = ApplyFilters(ventas, fechaInicio, fechaFin, detectedClient);
        var intent = ResolveIntent(request.Message, detectedClient);

        if (filteredSales.Count == 0)
        {
            var noDataAnswer = BuildNoDataAnswer(fechaInicio, fechaFin, detectedClient);
            _conversationMemory.AddMessage(conversationId, "user", request.Message);
            _conversationMemory.AddMessage(conversationId, "assistant", noDataAnswer);

            return new AppAiChatResponse
            {
                ConversationId = conversationId,
                Intent = intent,
                Answer = noDataAnswer,
                ClienteDetectado = detectedClient,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalVentas = 0,
                TotalImporte = 0,
                TotalBolsas = 0
            };
        }

        var topClientes = filteredSales
            .GroupBy(v => v.NombreCliente)
            .Select(group => new
            {
                Cliente = group.Key,
                Ventas = group.Count(),
                Bolsas = group.Sum(x => x.BolsasCompradas),
                Total = group.Sum(x => x.TotalPagar)
            })
            .OrderByDescending(x => x.Total)
            .Take(10)
            .ToList();

        var ventasPorDia = filteredSales
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

        var metodosPago = filteredSales
            .GroupBy(v => v.MetodoPago)
            .Select(group => new
            {
                MetodoPago = group.Key,
                Ventas = group.Count(),
                Total = group.Sum(x => x.TotalPagar)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        var detalleReciente = filteredSales
            .OrderByDescending(v => v.FechaEntrega)
            .Take(30)
            .Select(v => new
            {
                v.NombreCliente,
                v.NombreRepartidor,
                v.BolsasCompradas,
                v.TotalPagar,
                FechaEntrega = v.FechaEntrega?.ToString("yyyy-MM-dd HH:mm:ss"),
                v.MetodoPago
            })
            .ToList();

        var totalVentas = filteredSales.Count;
        var totalImporte = filteredSales.Sum(v => v.TotalPagar);
        var totalBolsas = filteredSales.Sum(v => v.BolsasCompradas);

        var prompt = BuildPrompt(
            request.Message.Trim(),
            request.Modulo,
            intent,
            fechaInicio,
            fechaFin,
            detectedClient,
            totalVentas,
            totalImporte,
            totalBolsas,
            topClientes,
            ventasPorDia,
            metodosPago,
            detalleReciente);

        var chatRequest = new OpenAiChatRequest
        {
            SystemPrompt =
                "Eres el asistente de la aplicacion VAAO. Tu trabajo es responder preguntas del negocio usando " +
                "solo el contexto entregado por el backend. No inventes cifras ni operaciones. " +
                "Si el usuario pide algo fuera del contexto disponible, indicalo claramente. " +
                "Responde en espanol, con tono profesional, breve y accionable.",
            Prompt = prompt,
            History = _conversationMemory.GetHistory(conversationId).ToList(),
            MaxTokens = 600
        };

        var aiResponse = await _azureOpenAiService.CreateChatCompletionAsync(chatRequest, cancellationToken);

        _conversationMemory.AddMessage(conversationId, "user", request.Message);
        _conversationMemory.AddMessage(conversationId, "assistant", aiResponse.Answer);

        return new AppAiChatResponse
        {
            ConversationId = conversationId,
            Intent = intent,
            Answer = aiResponse.Answer,
            ClienteDetectado = detectedClient,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            TotalVentas = totalVentas,
            TotalImporte = totalImporte,
            TotalBolsas = totalBolsas,
            Usage = aiResponse.Usage
        };
    }

    private static List<VentaView> ApplyFilters(
        List<VentaView> ventas,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? cliente)
    {
        var filtered = ventas.AsEnumerable();

        if (fechaInicio.HasValue || fechaFin.HasValue)
        {
            filtered = filtered.Where(v =>
                v.FechaEntrega.HasValue &&
                (!fechaInicio.HasValue || v.FechaEntrega.Value.Date >= fechaInicio.Value.Date) &&
                (!fechaFin.HasValue || v.FechaEntrega.Value.Date <= fechaFin.Value.Date));
        }

        if (!string.IsNullOrWhiteSpace(cliente))
        {
            filtered = filtered.Where(v => v.NombreCliente.Contains(cliente, StringComparison.OrdinalIgnoreCase));
        }

        return filtered.ToList();
    }

    private static string ResolveIntent(string message, string? detectedClient)
    {
        var normalized = message.ToLowerInvariant();

        if (normalized.Contains("mejor cliente") || normalized.Contains("top clientes") || normalized.Contains("clientes mas"))
        {
            return "top_clientes";
        }

        if (!string.IsNullOrWhiteSpace(detectedClient))
        {
            return "ventas_por_cliente";
        }

        if (normalized.Contains("compar") || normalized.Contains("tendencia") || normalized.Contains("comport"))
        {
            return "analisis_tendencia";
        }

        return "resumen_ventas";
    }

    private static string? DetectClientFromMessage(string message, List<string> clients)
    {
        var normalizedMessage = message.ToLowerInvariant();
        return clients.FirstOrDefault(client => normalizedMessage.Contains(client.ToLowerInvariant()));
    }

    private static (DateTime? FechaInicio, DateTime? FechaFin) ResolveDateRange(AppAiChatRequest request)
    {
        if (request.FechaInicio.HasValue || request.FechaFin.HasValue)
        {
            if (request.FechaInicio.HasValue && request.FechaFin.HasValue && request.FechaInicio.Value.Date > request.FechaFin.Value.Date)
            {
                throw new InvalidOperationException("La fechaInicio no puede ser mayor que la fechaFin.");
            }

            return (request.FechaInicio?.Date, request.FechaFin?.Date);
        }

        var today = DateTime.Today;
        var normalized = request.Message.ToLowerInvariant();

        if (normalized.Contains("hoy"))
        {
            return (today, today);
        }

        if (normalized.Contains("ayer"))
        {
            var yesterday = today.AddDays(-1);
            return (yesterday, yesterday);
        }

        if (normalized.Contains("esta semana"))
        {
            var diff = ((int)today.DayOfWeek + 6) % 7;
            var start = today.AddDays(-diff);
            return (start, today);
        }

        if (normalized.Contains("este mes"))
        {
            var start = new DateTime(today.Year, today.Month, 1);
            return (start, today);
        }

        if (normalized.Contains("mes pasado"))
        {
            var previousMonth = today.AddMonths(-1);
            var start = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return (start, end);
        }

        return (null, null);
    }

    private static string BuildNoDataAnswer(DateTime? fechaInicio, DateTime? fechaFin, string? cliente)
    {
        var parts = new List<string> { "No encontre ventas" };

        if (!string.IsNullOrWhiteSpace(cliente))
        {
            parts.Add($"para el cliente {cliente}");
        }

        if (fechaInicio.HasValue || fechaFin.HasValue)
        {
            parts.Add($"en el rango {FormatDateRange(fechaInicio, fechaFin)}");
        }

        return string.Join(" ", parts) + ".";
    }

    private static string BuildPrompt(
        string message,
        string modulo,
        string intent,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? cliente,
        int totalVentas,
        double totalImporte,
        int totalBolsas,
        object topClientes,
        object ventasPorDia,
        object metodosPago,
        object detalleReciente)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Modulo actual: {modulo}");
        builder.AppendLine($"Intent detectado: {intent}");
        builder.AppendLine($"Pregunta del usuario: {message}");
        builder.AppendLine($"Rango utilizado: {FormatDateRange(fechaInicio, fechaFin)}");
        builder.AppendLine($"Cliente detectado: {(string.IsNullOrWhiteSpace(cliente) ? "sin filtro" : cliente)}");
        builder.AppendLine($"Totales: ventas={totalVentas}, bolsas={totalBolsas}, importe={totalImporte:F2}");
        builder.AppendLine();
        builder.AppendLine("Resumen por cliente:");
        builder.AppendLine(JsonSerializer.Serialize(topClientes));
        builder.AppendLine();
        builder.AppendLine("Resumen por dia:");
        builder.AppendLine(JsonSerializer.Serialize(ventasPorDia));
        builder.AppendLine();
        builder.AppendLine("Resumen por metodo de pago:");
        builder.AppendLine(JsonSerializer.Serialize(metodosPago));
        builder.AppendLine();
        builder.AppendLine("Detalle reciente:");
        builder.AppendLine(JsonSerializer.Serialize(detalleReciente));
        builder.AppendLine();
        builder.AppendLine("Instrucciones de respuesta:");
        builder.AppendLine("- Responde solo usando los datos disponibles.");
        builder.AppendLine("- Si el usuario pide tendencias, compara dias o clientes cuando aplique.");
        builder.AppendLine("- Si el usuario pregunta por el mejor cliente, justifica con total e importe.");
        builder.AppendLine("- Si no hay evidencia suficiente para una conclusion, dilo claramente.");

        return builder.ToString();
    }

    private static string FormatDateRange(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var start = fechaInicio.HasValue ? fechaInicio.Value.ToString("yyyy-MM-dd") : "sin limite";
        var end = fechaFin.HasValue ? fechaFin.Value.ToString("yyyy-MM-dd") : "sin limite";
        return $"{start} a {end}";
    }
}
