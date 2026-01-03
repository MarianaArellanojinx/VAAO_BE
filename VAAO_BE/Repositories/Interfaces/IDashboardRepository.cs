namespace VAAO_BE.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        public Task<object> GetHistoricoVentas(DateTime start, DateTime end);
        public Task<object> GetEstatusPedidos(DateTime start, DateTime end);
    }
}
