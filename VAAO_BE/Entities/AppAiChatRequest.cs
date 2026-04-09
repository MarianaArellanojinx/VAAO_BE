namespace VAAO_BE.Entities;

public class AppAiChatRequest
{
    public string? ConversationId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Cliente { get; set; }
    public string Modulo { get; set; } = "ventas";
}
