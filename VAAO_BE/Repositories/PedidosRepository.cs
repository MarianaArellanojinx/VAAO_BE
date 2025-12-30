using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class PedidosRepository:IPedidoRepository
    {
        private readonly VAAOContext _context;

        public PedidosRepository(VAAOContext context)
        {
            _context = context;
        }


        private double CalcularTotalPagar(int totalBolsas, double precioUnitario)
        {
            return totalBolsas * precioUnitario;
        }


        public async Task CreatePedido(Pedidos payload)
        {
            try
            {
                payload.TotalPagar = CalcularTotalPagar(
                    payload.TotalBolsas,
                    payload.PrecioUnitario
                );

                payload.FechaPedido = DateTime.Now;

                await _context.Pedidos.AddAsync(payload);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        public async Task DeletePedido(int id)
        {
            try
            {
                var pedido = _context.Pedidos.Find(id);
                if (pedido is null) throw new Exception("Pedido no encontrado");

                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task<List<Pedidos>> GetAllPedidos()
        {
            try
            {
                var pedidos = await (
                    from p in _context.Pedidos
                    join c in _context.Clientes
                        on p.IdCliente equals c.IdCliente
                    select new Pedidos
                    {
                        IdPedido = p.IdPedido,
                        IdCliente = p.IdCliente,         
                        FechaPedido = p.FechaPedido,
                        FechaProgramada = p.FechaProgramada,
                        TotalBolsas = p.TotalBolsas,
                        PrecioUnitario = p.PrecioUnitario,
                        TotalPagar = p.TotalPagar,
                        EstatusPedido = p.EstatusPedido,
                        Observaciones = p.Observaciones,
                        IdRepartidor = p.IdRepartidor,
                        NombreCliente = c.NombreNegocio
                    }
                ).ToListAsync();

                return pedidos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task UpdatePedido(int id, Pedidos payload)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(id);
                if (pedido == null)
                    throw new Exception("Pedido no encontrado");

                pedido.IdCliente = payload.IdCliente;
                pedido.FechaPedido = payload.FechaPedido;
                pedido.FechaProgramada = payload.FechaProgramada;
                pedido.TotalBolsas = payload.TotalBolsas;
                pedido.PrecioUnitario = payload.PrecioUnitario;

               
                pedido.TotalPagar = CalcularTotalPagar(
                    payload.TotalBolsas,
                    payload.PrecioUnitario
                );

                pedido.EstatusPedido = payload.EstatusPedido;
                pedido.Observaciones = payload.Observaciones;
                pedido.IdRepartidor = payload.IdRepartidor;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
