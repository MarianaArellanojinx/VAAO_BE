using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _repo;

        public DashboardController(IDashboardRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetEstatusPedidos()
        {
            try
            {
                var result = await _repo.GetEstatusPedidos();
                return Ok(new
                {
                    data = result,
                    message = "",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetHistoricoVentas()
        {
            try
            {
                var result = await _repo.GetHistoricoVentas();
                return Ok(new
                {
                    data = result,
                    message = "",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
