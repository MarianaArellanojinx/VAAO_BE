using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using VAAO_BE.Data;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly VAAOContext _context;
        public DashboardRepository(VAAOContext context) => _context = context;

        private readonly int PEDIDO_PENDIENTE_APROBACION = 1;
        private readonly int APROBADO = 2;
        private readonly int CANCELADO = 3;

        private readonly int REPROGRAMADO = 1;
        private readonly int REPARTO = 2;
        private readonly int ENTREGADO = 3;

        public async Task<object> GetEstatusPedidos(DateTime s, DateTime e)
        {
            var pedidos = await _context.Pedidos
                .Where(x => x.FechaPedido >= s && x.FechaPedido <= e)
                .ToListAsync();
            var grouped = pedidos
                .GroupBy(x => x.FechaPedido.Date)
                .OrderBy(x => x.Key)
                .ToList();
            var labels = grouped
                .Select(g => g.Key.ToString("dd-MM-yyyy"))
                .ToList();
            var datasets = new[]
            {
                new
                {
                    label = "Pendiente aprobación",
                    data = grouped.Select(g => g.Count(p => p.EstatusPedido == PEDIDO_PENDIENTE_APROBACION)).ToList(),
                    borderRadius = 8,
                    backgroundColor = "#d7f032ff"
                },
                new
                {
                    label = "Aprobado",
                    data = grouped.Select(g => g.Count(p => p.EstatusPedido == APROBADO)).ToList(),
                    borderRadius = 8,
                    backgroundColor = "#4ef032ff"
                },
                new
                {
                    label = "Cancelado",
                    data = grouped.Select(g => g.Count(p => p.EstatusPedido == CANCELADO)).ToList(),
                    borderRadius = 8,
                    backgroundColor = "#f03235ff"
                }
            };

            return new
            {
                labels = labels,
                datasets = datasets
            };
        }


        public async Task<object> GetHistoricoVentas(DateTime s, DateTime end)
        {
            try
            {
                var today = DateTime.Now.Date.AddHours(6).AddMinutes(25);
                var start = today.AddDays(-30);
                var monday = GetMonday(today);
                var ventas = (await _context
                    .Ventas
                    .AsNoTracking()
                    .ToListAsync())
                    .AsEnumerable();
                var metodoPago = (await _context
                    .MetodosPago
                    .AsNoTracking()
                    .ToListAsync())
                    .AsEnumerable();
                var pedidos = (await _context
                    .Pedidos
                    .AsNoTracking()
                    .ToListAsync())
                    .AsEnumerable();
                var clientes = (await _context
                    .Clientes
                    .AsNoTracking()
                    .ToListAsync())
                    .AsEnumerable();
                var entregas = (await _context
                    .Entregas
                    .AsNoTracking()
                    .Where(x => x.FechaEntrega >= s && x.FechaEntrega <= end)
                    .ToListAsync()).AsEnumerable();
                var aux = from e in entregas
                          join p in pedidos
                          on e.IdPedido equals p.IdPedido
                          join v in ventas
                          on p.IdPedido equals v.IdPedido
                          join m in metodoPago
                          on v.IdMetodoPago equals m.IdMetodoPago
                          join c in clientes
                          on p.IdCliente equals c.IdCliente
                          select new
                          {
                              cliente = c.NombreCliente,
                              negocio = c.NombreNegocio,
                              bolsas = p.TotalBolsas,
                              total = p.TotalPagar,
                              pedido = p.FechaPedido,
                              metodo = m.Descripcion,
                              entregado = e.FechaEntrega
                          };
                var auxVentas = (from v in ventas
                                join p in pedidos
                                on v.IdPedido equals p.IdPedido
                                join e in entregas
                                on p.IdPedido equals e.IdPedido
                                where e.FechaEntrega >= s && e.FechaEntrega <= end
                                select new
                                {
                                    total = p.TotalPagar,
                                    entrega = (e.FechaEntrega ?? new DateTime()).Date
                                }).ToList();
                return new
                {
                    cards = new
                    {
                        day = auxVentas
                                .Where(x => x.entrega == today.Date)
                                .Sum(x => x.total),
                        week = auxVentas
                                .Where(x => x.entrega >= monday && x.entrega <= today)
                                .Sum(x => x.total),
                        month = auxVentas
                                .Where(x => x.entrega >= new DateTime(today.Year, today.Month, 1, 0, 0, 0) && x.entrega <= today)
                                .Sum(x => x.total)
                    },
                    table = aux.ToList(),
                    chart = new
                    {
                        labels = aux
                            .OrderBy(x => x.entregado)
                            .GroupBy(x => (x.entregado ?? new DateTime()).Date)
                            .Select(x => x.Key.ToString("dd-MM-yyyy"))
                            .ToList(),
                        datasets = new[]
                        {
                            new
                            {
                                label = "Ventas",
                                data = aux
                                        .GroupBy(x => (x.entregado ?? new DateTime()).Date)
                                        .OrderBy(x => x.Key)
                                        .Select(g => g.Sum(x => x.bolsas))
                                        .ToList()
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DateTime GetMonday(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.Date.AddDays(-diff);
        }
    }
}
