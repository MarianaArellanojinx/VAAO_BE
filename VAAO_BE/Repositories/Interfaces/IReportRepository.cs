namespace VAAO_BE.Repositories.Interfaces
{
    public interface IReportRepository
    {

        public Task<object> ObtenerReporteVentaPerdida();
        public Task<object> ObtenerReporteVentaRechazada();

    }
}
