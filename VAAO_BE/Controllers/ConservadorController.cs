using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class ConservadorController:ControllerBase
    {
        private readonly IConservadorRepository _conservadorRepository;
        public ConservadorController(IConservadorRepository conservadorRepository)
        {
            _conservadorRepository = conservadorRepository;
        }

        [HttpPost]
        public async Task<IActionResult> InsertConservador(Conservadores payload)
        {
            await _conservadorRepository.CreateConservador(payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetConservadores()
        {
            var result = await _conservadorRepository.GetAllConservadores();
            return Ok(new
            {
                data = result,
                message = string.Empty,
                status = true
            });
        }
    }
}
