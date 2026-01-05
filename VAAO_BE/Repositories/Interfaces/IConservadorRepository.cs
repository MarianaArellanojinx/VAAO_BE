
using VAAO_BE.Entities;


namespace VAAO_BE.Repositories.Interfaces
{
    public interface IConservadorRepository
    {
        public Task CreateConservador (Conservadores payload);
        public Task<List<Conservadores>> GetAllConservadores();
    }
}