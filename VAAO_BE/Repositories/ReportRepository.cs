using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VAAO_BE.Data;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly VAAOContext _context;

        public ReportRepository(VAAOContext context)
        {
            _context = context;
        }

        public DateTime ObtenerPrimerLunesDelAno(int anio)
        {
            DateTime primerDia = new DateTime(anio, 1, 1);

            int diasHastaLunes = ((int)DayOfWeek.Monday - (int)primerDia.DayOfWeek + 7) % 7;

            return primerDia.AddDays(diasHastaLunes);
        }

        public  int ObtenerSemanaDelAno(DateTime fecha)
        {
            var cultura = CultureInfo.InvariantCulture;

            return cultura.Calendar.GetWeekOfYear(
                fecha,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday
            );
        }
        public async Task<object> ObtenerReporteVentaPerdida()
        {
            var clientes = await _context.Clientes.ToListAsync();
            var pedidos = await _context.Pedidos.ToListAsync();
            var ventas = await _context.Ventas.ToListAsync();
            var aux = (
                from c in clientes
                join p in pedidos
                    on c.IdCliente equals p.IdCliente into cp
                from p in cp.DefaultIfEmpty()
                join v in ventas
                    on p?.IdPedido equals v.IdPedido into pv
                from v in pv.DefaultIfEmpty()
                select new
                {
                    cliente = c.NombreCliente,
                    bolsas = p?.TotalBolsas ?? 0,
                    fechaventa = v?.FechaRegistro
                }
            ).ToList();
            var auxgrouped = aux
                .GroupBy(x => new
                {
                    cliente = x.cliente,
                    semana = x.fechaventa != null
                        ? ObtenerSemanaDelAno(x.fechaventa.Value)
                        : (int?)null
                })
                .Select(x => new
                {
                    nombrecliente = x.Key.cliente,
                    numsemana = x.Key.semana,
                    totalbolsas = x.Sum(y => y.bolsas),
                    cumplio = x.Sum(y => y.bolsas) >= 30
                })
                .ToList();
            var clientesnocumplidos = auxgrouped
                .Where(x => x.totalbolsas < 30)
                .ToList();
            return clientesnocumplidos;
        }

        public async Task<object> ObtenerReporteVentaRechazada()
        {
            var pedidos = await _context.Pedidos.ToListAsync();
            var clientes = await _context.Clientes.ToListAsync();
            var result = (from p in pedidos
                         join c in clientes
                         on p.IdCliente equals c.IdCliente
                         where p.EstatusPedido == 3
                         select new
                         {
                             fechaPedido = p.FechaPedido,
                             cliente = c.NombreCliente.ToUpper(),
                             bolsas = p.TotalBolsas,
                             total = p.TotalPagar
                         }).ToList();
            return result
                .GroupBy(x => new { fecha = ObtenerSemanaDelAno(x.fechaPedido), cliente = x.cliente })
                .Select(x => new
                {
                    fechaPedido = x.Key.fecha,
                    cliente = x.Key.cliente,
                    totalBolsas = x.Sum(y => y.bolsas),
                    totalPagar = x.Sum(y => y.total)
                }).ToList();
        }
    }
}
