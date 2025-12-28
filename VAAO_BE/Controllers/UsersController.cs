using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _usersRepository.GetAllUsers();
            return Ok(new
            {
                data = result,
                message = string.Empty,
                status = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> InsertUsers(Users payload)
        {
            await _usersRepository.CreateUser(payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(int id, Users payload)
        {
            await _usersRepository.UpdateUser(id, payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _usersRepository.DeleteUser(id);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }
    }
}
