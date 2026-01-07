using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class EntregasRepository : IEntregasRepository
    {
        private readonly VAAOContext _context;

        public EntregasRepository(VAAOContext context)
        {
            _context = context;
        }

        public async Task CreateEntrega(Entregas payload)
        {
            try
            {
                if (payload.HoraInicio is not null)
                {    
                payload.HoraInicio = payload.HoraInicio.Value.AddHours(-6);
                }
                payload.HoraRegreso = null;
                payload.FechaEntrega = null;
                await _context.Entregas.AddAsync(payload);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task DeleteEntrega(int id)
        {
            try
            {
                var entrega = await _context.Entregas.FindAsync(id);
                if (entrega is null)
                    throw new Exception("Entrega no encontrada");

                _context.Entregas.Remove(entrega);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<object?> GetDetailOrder(int pedidoId)
        {
            try
            {
                var entregas = await _context.Entregas.ToListAsync();
                var pedidos = await _context.Pedidos.ToListAsync();
                var result = from e in entregas
                             join p in pedidos
                             on e.IdPedido equals p.IdPedido
                             where p.IdPedido == pedidoId
                             select new
                             {
                                 e.IdEntrega,
                                 e.IdRepartidor,
                                 e.IdPedido,
                                 e.FechaEntrega,
                                 e.HoraInicio,
                                 e.HoraLlegada,
                                 e.EstatusReparto,
                                 e.ImagenConservadorLlegada,
                                 e.ImagenConservadorSalida,
                             };
                return result?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Entregas>> GetAllEntregas()
        {
            try
            {
                return await _context.Entregas.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task UpdateEntrega(int id, Entregas payload, bool entregaFlag = false)
        {
            try
            {
                var entrega = await _context.Entregas.FindAsync(id);
                if (entrega == null)
                    throw new Exception("Entrega no encontrada");

                entrega.IdRepartidor = payload.IdRepartidor;
                entrega.IdPedido = payload.IdPedido;
                entrega.EstatusReparto = payload.EstatusReparto;
                entrega.Observaciones = payload.Observaciones;

                if (payload.HoraLlegada is not null && entregaFlag == false)
                {
                    entrega.HoraLlegada = payload.HoraLlegada.Value.AddHours(-6);
                }
                if(payload.FechaEntrega is not null && entregaFlag == true)
                {
                    entrega.FechaEntrega = payload.FechaEntrega.Value.AddHours(-6);
                }

                //if (payload.HoraRegreso is not null)
                //{
                //    entrega.HoraRegreso = payload.HoraRegreso.Value.AddHours(-6);
                //}

                entrega.ImagenConservadorLlegada = payload.ImagenConservadorLlegada;
                entrega.ImagenConservadorSalida = payload.ImagenConservadorSalida;
                entrega.ImagenIncidenciaConservador = payload.ImagenIncidenciaConservador;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
