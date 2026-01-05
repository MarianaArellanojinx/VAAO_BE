using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class ConservadorRepository : IConservadorRepository
    {
        private readonly VAAOContext _context;
        public ConservadorRepository(VAAOContext context) => _context = context;

        public async Task CreateConservador(Conservadores payload)
        {
            try
            {
                _context.Conservadores.Add(payload);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating conservador", ex);
            }
        }

        public async Task<object> GetAllConservadores()
        {
            try
            {
                var clients = await _context.Clientes.ToListAsync();
                var conservadores = await _context.Conservadores.ToListAsync();
                var cliente_conservador = await _context.Cliente_Conservador.ToListAsync();
                var result = from cc in cliente_conservador
                join c in clients on cc.IdCliente equals c.IdCliente
                join con in conservadores on cc.IdConservador equals con.IdConservador
                select new
                {
                    Cliente = c.NombreCliente,
                    Negocio = c.NombreNegocio,
                    Conservador = con.SerialNumber,
                    Estatus = con.EstatusConservador
                };
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving conservadores", ex);
            }
        }
    }
}
