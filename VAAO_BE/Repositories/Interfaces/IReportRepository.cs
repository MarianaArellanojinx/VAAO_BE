using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<List<ReporteVentaPerdidaDetalleDto>> ObtenerReporteVentaPerdida(DateTime startDate, DateTime endDate, int metaBolsas);
        Task<ReporteVentaPerdidaResponseDto> ObtenerReporteVentaPerdidaResumen(DateTime startDate, DateTime endDate, int metaBolsas);
        Task<object> ObtenerReporteVentaRechazada(DateTime startDate, DateTime endDate);
    }
}
