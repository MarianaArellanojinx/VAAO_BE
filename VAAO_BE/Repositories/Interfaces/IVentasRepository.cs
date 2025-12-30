using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IVentasRepository
    {
        Task CreateVenta(Ventas payload);

        Task<List<VentaView>> GetVentas();

    }
}
