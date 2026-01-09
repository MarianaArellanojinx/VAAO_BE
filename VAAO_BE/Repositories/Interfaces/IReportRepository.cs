namespace VAAO_BE.Repositories.Interfaces
{
    public interface IReportRepository
    {

        public Task<object> ObtenerReporteVentaPerdida(DateTime startDate, DateTime endDate);
        public Task<object> ObtenerReporteVentaRechazada(DateTime startDate, DateTime endDate);

    }
}
