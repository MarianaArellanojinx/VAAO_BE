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
                payload.FechaEntrega = DateTime.Now.AddHours(-6);
                payload.HoraInicio = null;
                payload.HoraRegreso = null;

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

        public async Task UpdateEntrega(int id, Entregas payload)
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
                entrega.HoraInicio = payload.HoraInicio.Value.AddHours(-6);
                entrega.HoraLlegada = payload.HoraLlegada.Value.AddHours(-6);
                entrega.HoraRegreso = payload.HoraRegreso.Value.AddHours(-6);

               
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
