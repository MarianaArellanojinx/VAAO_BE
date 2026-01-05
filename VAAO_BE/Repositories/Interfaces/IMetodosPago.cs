using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IMetodosPago
    {
        public Task CreateMetodoPago(MetodosPago payload);
        public Task<List<MetodosPago>> GetAllMetodosPago();
    }
}
