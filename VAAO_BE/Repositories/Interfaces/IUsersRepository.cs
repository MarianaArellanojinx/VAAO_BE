using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        public Task CreateUser(Users payload);
        public Task UpdateUser(int id, Users payload);
        public Task DeleteUser(int id);
        public Task<List<Users>> GetAllUsers();
        public Task<Users> Login(string userName, string password);

    }

}
