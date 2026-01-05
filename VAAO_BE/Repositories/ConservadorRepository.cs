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
                var result =
                            from con in conservadores
                            join cc in cliente_conservador
                                on con.IdConservador equals cc.IdConservador into ccJoin
                            from cc in ccJoin.DefaultIfEmpty()

                            join c in clients
                                on (cc != null ? (int?)cc.IdCliente : null)
                                equals (int?)c.IdCliente into cJoin
                            from c in cJoin.DefaultIfEmpty()

                            select new
                            {
                                Negocio = c?.NombreNegocio,
                                Cliente = c?.NombreCliente,
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
