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

        public int ObtenerSemanaDelAno(DateTime fecha)
        {
            var cultura = CultureInfo.InvariantCulture;

            return cultura.Calendar.GetWeekOfYear(
                fecha,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday
            );
        }
        public async Task<object> ObtenerReporteVentaPerdida(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            // Generamos las semanas válidas dentro del rango
            var semanasValidas = Enumerable
                .Range(0, (endDate - startDate).Days + 1)
                .Select(d => startDate.AddDays(d))
                .Select(f => ObtenerSemanaDelAno(f))
                .Distinct()
                .ToList();

            var clientes = await _context.Clientes.ToListAsync();
            var pedidos = await _context.Pedidos.ToListAsync();
            var ventas = await _context.Ventas
                .Where(v =>
                    v.FechaRegistro != null &&
                    v.FechaRegistro.Date >= startDate &&
                    v.FechaRegistro.Date <= endDate
                )
                .ToListAsync();

            var aux =
                (
                    from c in clientes
                    from semana in semanasValidas                 // cliente × semana
                    join p in pedidos
                        on c.IdCliente equals p.IdCliente into cp
                    from p in cp.DefaultIfEmpty()
                    join v in ventas
                        on p?.IdPedido equals v.IdPedido into pv
                    from v in pv.DefaultIfEmpty()
                    where v == null || ObtenerSemanaDelAno(v.FechaRegistro) == semana
                    select new
                    {
                        cliente = c.NombreCliente,
                        semana = semana,
                        negocio = c.NombreNegocio,
                        pago = p.TotalPagar,
                        bolsas = v != null ? p?.TotalBolsas ?? 0 : 0
                    }
                )
                .ToList();

            var resultado = aux
                .GroupBy(x => new
                {
                    x.cliente,
                    x.semana,
                    x.negocio
                })
                .Select(g => new
                {
                    negocio = g.Key.negocio,
                    nombrecliente = g.Key.cliente,
                    numsemana = g.Key.semana,
                    totalbolsas = g.Sum(x => x.bolsas),
                    totalPagar = g.Sum(p => p.pago),
                    cumplio = g.Sum(x => x.bolsas) >= 30
                })
                .OrderBy(x => x.numsemana)
                .ToList();

            return resultado;
        }


        public async Task<object> ObtenerReporteVentaRechazada(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            // Semanas válidas dentro del rango
            var semanasValidas = Enumerable
                .Range(0, (endDate - startDate).Days + 1)
                .Select(d => startDate.AddDays(d))
                .Select(f => ObtenerSemanaDelAno(f))
                .Distinct()
                .ToList();

            var pedidos = await _context.Pedidos
                .Where(p =>
                    p.EstatusPedido == 3 &&
                    p.FechaPedido.Date >= startDate &&
                    p.FechaPedido.Date <= endDate
                )
                .ToListAsync();

            var clientes = await _context.Clientes.ToListAsync();

            var aux =
                (
                    from c in clientes
                    from semana in semanasValidas                    // cliente × semana
                    join p in pedidos
                        on c.IdCliente equals p.IdCliente into cp
                    from p in cp.DefaultIfEmpty()
                    where p == null || ObtenerSemanaDelAno(p.FechaPedido) == semana
                    select new
                    {
                        cliente = c.NombreCliente.ToUpper(),
                        negocio = c.NombreNegocio,
                        semana = semana,
                        bolsas = p != null ? p.TotalBolsas : 0,
                        total = p != null ? p.TotalPagar : 0
                    }
                )
                .ToList();

            var resultado = aux
                .GroupBy(x => new
                {
                    x.cliente,
                    x.negocio,
                    x.semana
                })
                .Select(g => new
                {
                    cliente = g.Key.cliente,
                    negocio = g.Key.negocio,
                    numsemana = g.Key.semana,
                    totalBolsas = g.Sum(x => x.bolsas),
                    totalPagar = g.Sum(x => x.total)
                })
                .OrderBy(x => x.numsemana)
                .ToList();

            return resultado;
        }

    }
}
