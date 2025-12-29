using Microsoft.EntityFrameworkCore;
using VAAO_BE.Data;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Repositories
{
    public class UserRepository : IUsersRepository
    {
        private readonly VAAOContext _context;

        public UserRepository(VAAOContext context)
        {

            _context = context;
        }
        public async Task CreateUser(Users payload)
        {
            try
            {
                await _context.AddAsync(payload);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.Find(id);
                if (user is null) throw new Exception("usuario no encontrado");
                user.IsActive = false;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task<List<Users>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task UpdateUser(int id, Users payload)
        {
            try
            {
                var user = _context.Users.Find(id);
                if (user is null) throw new Exception("usuario no encontrado");
                user.UserPassword = payload.UserPassword;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task<Users> Login(string userName, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (user == null)
                    throw new Exception("Usuario no encontrado");

                if (!user.IsActive)
                    throw new Exception("Usuario inactivo");

               
                if (user.UserPassword != password)
                    throw new Exception("Contraseña incorrecta");

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


    }
}
