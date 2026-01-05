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
                var result = from con in conservadores
                            join cc in cliente_conservador
                                on con.IdConservador equals cc.IdConservador into ccJoin
                            from cc in ccJoin.DefaultIfEmpty() // LEFT JOIN cliente_conservador

                            join c in clients
                                on cc.IdCliente equals c.IdCliente into cJoin
                            from c in cJoin.DefaultIfEmpty()   // LEFT JOIN clients

                            select new
                            {
                                Negocio = c != null ? c.NombreNegocio : null,
                                Cliente = c != null ? c.NombreCliente : null,
                                Serial = con.SerialNumber,
                                Estatus = con.EstatusConservador == 1 ? "Activo" : "Inactivo",
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
