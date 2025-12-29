
using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{


    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RepartidoresController:ControllerBase
        
    {
        private readonly IRepartidoresRepository _repartidoresRepository;
        public RepartidoresController(IRepartidoresRepository repartidoresRepository)
        {
            _repartidoresRepository = repartidoresRepository;
        }

        [HttpPost]
        public async Task<IActionResult> InsertRepartidores(Repartidores payload)
        {
            await _repartidoresRepository.CreateRepartidor(payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetRepartidores()
        {
            var result = await _repartidoresRepository.GetAllRepartidores();
            return Ok(new
            {
                data = result,
                message = string.Empty,
                status = true
            });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateRepartidor(int id, Repartidores payload)
        {
            await _repartidoresRepository.UpdateRepartidor(id, payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRepartidores(int id)
        {
            await _repartidoresRepository.DeleteRepartidor(id);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }



    }
}
