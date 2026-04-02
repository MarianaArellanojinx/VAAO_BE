namespace VAAO_BE.Entities;

public class VentasAiSummaryRequest
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Cliente { get; set; }
    public string? Pregunta { get; set; }
}
