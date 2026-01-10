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

        [HttpGet]
        public async Task<IActionResult> GetDetail(int pedidoId)
        {
            var result = await _entregasRepository.GetDetailOrder(pedidoId);
            return Ok(new
            {
                data = result,
                message = "",
                status = true
            });
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
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpPatch("UpdateEntrega/{id}/{entrega}")]
        public async Task<IActionResult> UpdateEntrega(int id, [FromBody] Entregas payload, bool entrega = false)
        {
            try
            {
                await _entregasRepository.UpdateEntrega(id, payload, entrega);
                return Ok(new
                {
                    data = true,
                    message = string.Empty,
                    status = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteEntrega/{id}")]
        public async Task<IActionResult> DeleteEntrega(int id)
        {
            await _entregasRepository.DeleteEntrega(id);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }


    }
}
