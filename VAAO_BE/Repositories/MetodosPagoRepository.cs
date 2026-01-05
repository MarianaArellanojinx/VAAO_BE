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

        public Task CreateMetodoPago(MetodosPagoRepository payload)
        {
            throw new NotImplementedException();
        }

        public Task<List<MetodosPagoRepository>> GetAllMetodosPago()
        {
            throw new NotImplementedException();
        }
    }
}
