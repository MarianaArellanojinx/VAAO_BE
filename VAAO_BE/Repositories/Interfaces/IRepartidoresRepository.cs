

using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IRepartidoresRepository
    {
        public Task CreateRepartidor(Repartidores payload);
        public Task UpdateRepartidor(int id, Repartidores payload);
        public Task DeleteRepartidor(int id);

        public Task<List<Repartidores>> GetAllRepartidores();
    }
}
