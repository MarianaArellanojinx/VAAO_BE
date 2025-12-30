using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentasRepository _ventasRepository;

        public VentasController(IVentasRepository ventasRepository)
        {
            _ventasRepository = ventasRepository;
        }

        [HttpPost]
        public async Task<IActionResult> InsertVenta([FromBody] Ventas payload)
        {
            await _ventasRepository.CreateVenta(payload);

            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetVentas()
        {
            var ventas = await _ventasRepository.GetVentas();

            return Ok(new
            {
                data = ventas,
                message = string.Empty,
                status = true
            });
        }
    }
}
