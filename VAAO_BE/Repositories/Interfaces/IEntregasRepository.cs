using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IEntregasRepository
    {
        public Task<object?> GetDetailOrder(int pedidoId);
        public Task CreateEntrega(Entregas payload);
        public Task UpdateEntrega(int id, Entregas payload, bool entregaFlag = false);
        public Task DeleteEntrega(int id);

        public Task<List<Entregas>> GetAllEntregas();
    }
}
