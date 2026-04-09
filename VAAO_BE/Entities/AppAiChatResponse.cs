namespace VAAO_BE.Entities;

public class AppAiChatResponse
{
    public string ConversationId { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? ClienteDetectado { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int TotalVentas { get; set; }
    public double TotalImporte { get; set; }
    public int TotalBolsas { get; set; }
    public OpenAiUsage Usage { get; set; } = new();
}
