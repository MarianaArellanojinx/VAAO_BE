using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IMetodosPago
    {
        public Task CreateMetodoPago(MetodosPagoRepository payload);
        public Task<List<MetodosPagoRepository>> GetAllMetodosPago();
    }
}
