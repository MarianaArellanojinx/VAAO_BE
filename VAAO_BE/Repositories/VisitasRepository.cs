using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class VisitasRepository : IVisitasRepository
    {
        private readonly VAAOContext _context;

        public VisitasRepository(VAAOContext context) => _context = context;

        public async Task<object> GetVisitas(DateTime day)
        {
            try
            {
                var clients = await _context.Clientes.ToListAsync();
                var visitas = await _context.Visitas.ToListAsync();
                var users = await _context.Users.ToListAsync();
                var result = from v in visitas
                             join c in clients
                             on v.IdCliente equals c.IdCliente
                             join u in users
                             on v.IdUsuario equals u.IdUser
                             select new
                             {
                                 Fecha = v.FechaVisita,
                                 Evidencia = v.Evidencia,
                                 Cliente = c.NombreNegocio,
                                 Encargado = u.UserName
                             };
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task InsertVisita(Visitas payload)
        {
            try
            {
                await _context.Visitas.AddAsync(payload);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
