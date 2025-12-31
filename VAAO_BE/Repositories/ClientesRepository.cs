using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class ClientesRepository : IClientesRepository
    {

        private readonly VAAOContext _context;

        public ClientesRepository(VAAOContext context)
        {
            _context = context;
        }


        public async Task CreateCliente(Clientes payload)
        {
            try
            {
                payload.FechaAlta = DateTime.Now.AddHours(-6);
                payload.FechaBaja = null;

                await _context.Clientes.AddAsync(payload);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        public async Task DeleteCliente(int id)
        {
            try
            {
                var cliente = _context.Clientes.Find(id);
                if (cliente is null) throw new Exception("cliente no encontrado");
                
                 _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task<List<Clientes>> GetAllClientes()
        {

            try
            {
                var clientes = await _context.Clientes.ToListAsync();
                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task UpdateCliente(int id, Clientes payload)
        {
            try
            {
                var cliente =  _context.Clientes.Find(id);
                if (cliente == null) throw new Exception("cliente no encontrado");


                cliente.NombreNegocio = payload.NombreNegocio;
                cliente.NombreCliente = payload.NombreCliente;
                cliente.Calle = payload.Calle;
                cliente.Colonia = payload.Colonia;
                cliente.NumeroExterior = payload.NumeroExterior;
                cliente.Cp = payload.Cp;
                cliente.Telefono = payload.Telefono;
                cliente.Conservadores = payload.Conservadores;
                    
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }
    }
}
