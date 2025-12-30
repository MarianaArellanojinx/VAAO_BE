using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntregasController : ControllerBase
    {
        private readonly IEntregasRepository _entregasRepository;

        public EntregasController(
            IEntregasRepository entregasRepository)
        {
            _entregasRepository = entregasRepository;
        }
    
        [HttpGet("GetAllEntregas")]
        public async Task<IActionResult> GetAllEntregas()
        {
            var entregas = await _entregasRepository.GetAllEntregas();
            return Ok(new
            {
                data = entregas,
                message = "",
                status = true
            });
        }

        [HttpPost("CreateEntrega")]
        public async Task<IActionResult> CreateEntrega([FromBody] Entregas payload)
        {
            await _entregasRepository.CreateEntrega(payload);
            return Ok("Entrega creada correctamente");
        }

        [HttpPut("UpdateEntrega/{id}")]
        public async Task<IActionResult> UpdateEntrega(int id, [FromBody] Entregas payload)
        {
            await _entregasRepository.UpdateEntrega(id, payload);
            return Ok("Entrega actualizada correctamente");
        }

        [HttpDelete("DeleteEntrega/{id}")]
        public async Task<IActionResult> DeleteEntrega(int id)
        {
            await _entregasRepository.DeleteEntrega(id);
            return Ok("Entrega eliminada correctamente");
        }


    }
}
