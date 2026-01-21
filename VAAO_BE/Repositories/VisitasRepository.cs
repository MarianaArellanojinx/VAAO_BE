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
                var start = day.Date.AddHours(6).AddMinutes(25);
                var end = start.AddDays(1).AddSeconds(-1);

                var result = await
                (
                    from c in _context.Clientes

                    join v in _context.Visitas
                        .Where(x => x.FechaVisita >= start && x.FechaVisita <= end)
                        on c.IdCliente equals v.IdCliente into visitasGroup
                    from v in visitasGroup.DefaultIfEmpty()

                    join u in _context.Users
                        on v.IdUsuario equals u.IdUser into usersGroup
                    from u in usersGroup.DefaultIfEmpty()

                    select new
                    {
                        Fecha = v != null ? v.FechaVisita : (DateTime?)null,
                        Evidencia = v != null ? v.Evidencia : null,
                        Cliente = c.NombreNegocio,
                        Encargado = u != null ? u.UserName : null
                    }
                ).ToListAsync();

                return result;
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
                payload.FechaVisita = DateTime.Now;
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
