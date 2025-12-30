namespace VAAO_BE.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        public Task<object> GetHistoricoVentas();
        public Task<object> GetEstatusPedidos();
    }
}
