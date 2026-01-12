
using VAAO_BE.Entities;


namespace VAAO_BE.Repositories.Interfaces
{
    public interface IClientesRepository
    {
        public Task CreateCliente (Clientes payload);
        public Task UpdateCliente(int id, Clientes payload);
        public Task DeleteCliente(int id);

        public Task<object> GetAllClientes();
    }
}
