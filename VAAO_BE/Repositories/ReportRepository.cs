using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly VAAOContext _context;
        private const int MetaBolsasDefault = 30;

        public ReportRepository(VAAOContext context)
        {
            _context = context;
        }

        private int ObtenerSemanaDelAno(DateTime fecha)
        {
            var cultura = CultureInfo.InvariantCulture;
            return cultura.Calendar.GetWeekOfYear(
                fecha,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday
            );
        }

        private static DateTime ObtenerInicioSemana(DateTime fecha)
        {
            var diferencia = ((int)fecha.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return fecha.Date.AddDays(-diferencia);
        }

        private static List<(int Anio, int Semana, DateTime Inicio, DateTime Fin)> ObtenerSemanasEnRango(DateTime startDate, DateTime endDate)
        {
            return Enumerable
                .Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset))
                .GroupBy(dia => new
                {
                    Anio = ISOWeek.GetYear(dia),
                    Semana = ISOWeek.GetWeekOfYear(dia)
                })
                .Select(group =>
                {
                    var inicio = ObtenerInicioSemana(group.Min());
                    return (
                        group.Key.Anio,
                        group.Key.Semana,
                        inicio,
                        inicio.AddDays(6)
                    );
                })
                .OrderBy(semana => semana.Anio)
                .ThenBy(semana => semana.Semana)
                .ToList();
        }

        private async Task<List<ReporteVentaPerdidaDetalleDto>> ConstruirReporteVentaPerdida(DateTime startDate, DateTime endDate, int metaBolsas)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            if (endDate < startDate)
            {
                throw new ArgumentException("La fecha final no puede ser menor a la fecha inicial.");
            }

            var meta = metaBolsas > 0 ? metaBolsas : MetaBolsasDefault;
            var semanas = ObtenerSemanasEnRango(startDate, endDate);
            var endDateExclusive = endDate.AddDays(1);

            if (semanas.Count == 0)
            {
                return [];
            }

            var clientes = await _context.Clientes
                .Select(cliente => new
                {
                    cliente.IdCliente,
                    cliente.NombreCliente,
                    cliente.NombreNegocio
                })
                .ToListAsync();

            if (clientes.Count == 0)
            {
                return [];
            }

            var ventasConcretadas = await (
                from venta in _context.Ventas
                join pedido in _context.Pedidos on venta.IdPedido equals pedido.IdPedido
                where venta.FechaRegistro >= startDate
                      && venta.FechaRegistro < endDateExclusive
                select new
                {
                    pedido.IdCliente,
                    pedido.TotalBolsas,
                    pedido.TotalPagar,
                    venta.FechaRegistro
                }).ToListAsync();

            var comprasPorClienteSemana = ventasConcretadas
                .GroupBy(item => new
                {
                    item.IdCliente,
                    Anio = ISOWeek.GetYear(item.FechaRegistro),
                    Semana = ISOWeek.GetWeekOfYear(item.FechaRegistro)
                })
                .ToDictionary(
                    group => $"{group.Key.IdCliente}-{group.Key.Anio}-{group.Key.Semana}",
                    group => (
                        BolsasCompradas: group.Sum(item => item.TotalBolsas),
                        TotalPagado: group.Sum(item => item.TotalPagar)
                    )
                );

            var detalle = new List<ReporteVentaPerdidaDetalleDto>(clientes.Count * semanas.Count);

            foreach (var cliente in clientes)
            {
                foreach (var semana in semanas)
                {
                    var key = $"{cliente.IdCliente}-{semana.Anio}-{semana.Semana}";
                    var bolsasCompradas = 0;
                    var totalPagado = 0d;

                    if (comprasPorClienteSemana.TryGetValue(key, out var compra))
                    {
                        bolsasCompradas = compra.BolsasCompradas;
                        totalPagado = compra.TotalPagado;
                    }

                    var bolsasPerdidas = Math.Max(0, meta - bolsasCompradas);

                    detalle.Add(new ReporteVentaPerdidaDetalleDto
                    {
                        IdCliente = cliente.IdCliente,
                        NombreCliente = cliente.NombreCliente,
                        NombreNegocio = cliente.NombreNegocio,
                        AnioSemana = semana.Anio,
                        NumeroSemana = semana.Semana,
                        InicioSemana = semana.Inicio,
                        FinSemana = semana.Fin,
                        BolsasObjetivo = meta,
                        BolsasCompradas = bolsasCompradas,
                        BolsasPerdidas = bolsasPerdidas,
                        TotalPagado = totalPagado,
                        CumplioObjetivo = bolsasCompradas >= meta
                    });
                }
            }

            return detalle;
        }

        public async Task<List<ReporteVentaPerdidaDetalleDto>> ObtenerReporteVentaPerdida(DateTime startDate, DateTime endDate, int metaBolsas)
        {
            var detalle = await ConstruirReporteVentaPerdida(startDate, endDate, metaBolsas);

            return detalle
                .OrderBy(item => item.AnioSemana)
                .ThenBy(item => item.NumeroSemana)
                .ThenBy(item => item.NombreNegocio)
                .ToList();
        }

        public async Task<ReporteVentaPerdidaResponseDto> ObtenerReporteVentaPerdidaResumen(DateTime startDate, DateTime endDate, int metaBolsas)
        {
            var detalle = await ConstruirReporteVentaPerdida(startDate, endDate, metaBolsas);
            var detalleOrdenado = detalle
                .OrderBy(item => item.AnioSemana)
                .ThenBy(item => item.NumeroSemana)
                .ThenBy(item => item.NombreNegocio)
                .ToList();

            var totalBolsasObjetivo = detalleOrdenado.Sum(item => item.BolsasObjetivo);
            var totalBolsasCompradas = detalleOrdenado.Sum(item => item.BolsasCompradas);
            var totalBolsasPerdidas = detalleOrdenado.Sum(item => item.BolsasPerdidas);

            var resumen = new ReporteVentaPerdidaResumenDto
            {
                MetaBolsasPorSemana = metaBolsas > 0 ? metaBolsas : MetaBolsasDefault,
                TotalNegocios = detalleOrdenado.Select(item => item.IdCliente).Distinct().Count(),
                TotalSemanas = detalleOrdenado.Select(item => $"{item.AnioSemana}-{item.NumeroSemana}").Distinct().Count(),
                TotalRegistros = detalleOrdenado.Count,
                TotalBolsasObjetivo = totalBolsasObjetivo,
                TotalBolsasCompradas = totalBolsasCompradas,
                TotalBolsasPerdidas = totalBolsasPerdidas,
                NegociosConFaltante = detalleOrdenado
                    .Where(item => item.BolsasPerdidas > 0)
                    .Select(item => item.IdCliente)
                    .Distinct()
                    .Count(),
                NegociosSinCompras = detalleOrdenado
                    .GroupBy(item => item.IdCliente)
                    .Count(group => group.Sum(item => item.BolsasCompradas) == 0),
                PorcentajeCumplimiento = totalBolsasObjetivo == 0
                    ? 0
                    : Math.Round((double)totalBolsasCompradas * 100 / totalBolsasObjetivo, 2)
            };

            return new ReporteVentaPerdidaResponseDto
            {
                Resumen = resumen,
                Detalle = detalleOrdenado
            };
        }

        public async Task<object> ObtenerReporteVentaRechazada(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

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
                    from semana in semanasValidas
                    join p in pedidos
                        on c.IdCliente equals p.IdCliente into cp
                    from p in cp.DefaultIfEmpty()
                    where p == null || ObtenerSemanaDelAno(p.FechaPedido) == semana
                    select new
                    {
                        cliente = c.NombreCliente.ToUpper(),
                        negocio = c.NombreNegocio,
                        semana,
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
