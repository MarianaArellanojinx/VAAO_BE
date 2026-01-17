using System.Threading.Tasks;
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


        private async Task<double> CalcularTotalPagar(int totalBolsas, int idCliente)
        {
            var cliente = await _context.Clientes.FindAsync(idCliente);
            if(cliente is null) return 25.0;
            return totalBolsas * (cliente?.PrecioHielo ?? 0.0);
        }


        public async Task CreatePedido(Pedidos payload)
        {
            try
            {
                payload.IdRepartidor = null;
                payload.FechaPedido = payload.FechaPedido.AddHours(-6);
                payload.FechaProgramada = payload.FechaProgramada.AddHours(-6);
                payload.EstatusPedido = 1;
                payload.TotalPagar = await CalcularTotalPagar(payload.TotalBolsas, payload.IdCliente);
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

        public async Task<List<PedidoDetailDto>> GetAllPedidos()
        {
            try
            {
                var pedidos = await (
                    from p in _context.Pedidos
                    join c in _context.Clientes
                        on p.IdCliente equals c.IdCliente
                    select new PedidoDetailDto
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
                        NombreCliente = c.NombreNegocio,
                        Ubicacion = c.Ubicacion
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

               
                pedido.TotalPagar = await CalcularTotalPagar(
                    payload.TotalBolsas,
                    payload.IdCliente
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
