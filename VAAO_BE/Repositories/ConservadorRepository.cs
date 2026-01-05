using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class ConservadorRepository : IConservadorRepository
    {
        private readonly VAAOContext _context;
        public ConservadorRepository(VAAOContext context) => _context = context;

        public async Task CreateConservador(Conservadores payload)
        {
            try
            {
                _context.Conservadores.Add(payload);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating conservador", ex);
            }
        }

        public async Task<List<Conservadores>> GetAllConservadores()
        {
            try
            {
                return await _context.Conservadores.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving conservadores", ex);
            }
        }
    }
}
