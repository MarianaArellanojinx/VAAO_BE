

using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IPedidoRepository
    {
        public Task CreatePedido(Pedidos payload);
        public Task UpdatePedido(int id, Pedidos payload);
        public Task DeletePedido(int id);

        public Task<object> GetAllPedidos();
    }
}
