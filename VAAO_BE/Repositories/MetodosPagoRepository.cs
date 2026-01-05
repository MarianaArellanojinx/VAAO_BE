using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class MetodosPagoRepository : IMetodosPago
    { 
        private readonly VAAOContext _context;

    public MetodosPagoRepository(VAAOContext context)
    {
        _context = context;
    }

 
        public Task CreateMetodoPago(MetodosPago payload)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MetodosPago>> GetAllMetodosPago()
        {
            try
            {
                var metodospago = await _context.MetodosPago.ToListAsync();
                return metodospago;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

      
    }
}
