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
            var ventas = await _context.Ventas.ToListAsync();
            var pedidos = await _context.Pedidos.ToListAsync();

            var aux = (from v in ventas
                       join p in pedidos
                       on v.IdPedido equals p.IdPedido
                       join c in clientes
                       on p.IdCliente equals c.IdCliente
                       select new
                       {
                           bolsas = p.TotalBolsas,
                           fechaventa = v.FechaRegistro,
                           cliente = c.NombreCliente
                       }).ToList();

            var auxgrouped = aux.GroupBy(x => new
            {
                semana = ObtenerSemanaDelAno(x.fechaventa),
                cliente = x.cliente
            })
                .Select(x => new
                {
                    nombrecliente = x.Key.cliente,
                    totalbolsas = x.Sum(y => y.bolsas),
                    numsemana = x.Key.semana

                }).ToList();

        }
    }
}
