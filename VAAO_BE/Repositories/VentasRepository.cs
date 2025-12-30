using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class VentasRepository : IVentasRepository
    {
        private readonly VAAOContext _context;

        public VentasRepository(VAAOContext context)
        {

            _context = context;
        }

        public async Task CreateVenta(Ventas payload)
        {
            await _context.Ventas.AddAsync(payload);
            await _context.SaveChangesAsync();
        }

        public async Task<List<VentaView>> GetVentas()
        {
            var ventas = await (
                from v in _context.Ventas

                join p in _context.Pedidos
                    on v.IdPedido equals p.IdPedido

                join c in _context.Clientes
                    on p.IdCliente equals c.IdCliente

                join r in _context.Repartidores
                    on p.IdRepartidor equals r.IdRepartidor
                    into repartidores
                from r in repartidores.DefaultIfEmpty()

                join e in _context.Entregas
                    on p.IdPedido equals e.IdPedido
                    into entregas
                from e in entregas.DefaultIfEmpty()

                join mp in _context.MetodosPago
                    on v.IdMetodoPago equals mp.IdMetodoPago

                select new VentaView
                {
                    IdVenta = v.IdVenta,
                    IdPedido = p.IdPedido,

                    NombreCliente = c.NombreNegocio,
                    NombreRepartidor = r != null ? r.NombreRepartidor : "Sin asignar",

                    BolsasCompradas = p.TotalBolsas,
                    TotalPagar = p.TotalPagar,

                    FechaEntrega = e.FechaEntrega,
                    MetodoPago = mp.Descripcion
                }
            ).ToListAsync();

            return ventas;
        }


    }
}

