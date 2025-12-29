using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;


namespace VAAO_BE.Repositories
{
    public class RepartidoresRepository:IRepartidoresRepository

    {
        private readonly VAAOContext _context;

        public RepartidoresRepository(VAAOContext context)
        {
            _context = context;
        }

        public async Task CreateRepartidor(Repartidores payload)
        {
            try
            {
                payload.AltaRepartidor = DateTime.Now;
                payload.BajaRepartidor = null;

                await _context.Repartidores.AddAsync(payload);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task DeleteRepartidor(int id)
        {
            try
            {
                var repartidor = _context.Repartidores.Find(id);
                if (repartidor is null) throw new Exception("Repartidor no encontrado");

                _context.Repartidores.Remove(repartidor);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task<List<Repartidores>> GetAllRepartidores()
        {
            try
            {
                var repartidor = await _context.Repartidores.ToListAsync();
                return repartidor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task UpdateRepartidor(int id, Repartidores payload)
        {

            try
            {
                var repartidor = _context.Repartidores.Find(id);
                if (repartidor == null) throw new Exception("Repartidor no encontrado");


                repartidor.NombreRepartidor = payload.NombreRepartidor;
                repartidor.ApellidoRepartidor = payload.ApellidoRepartidor;
             
                

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }
    }
}
