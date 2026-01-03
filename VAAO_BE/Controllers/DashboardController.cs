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
        public async Task<IActionResult> GetEstatusPedidos(DateTime start, DateTime end)
        {
            try
            {
                start = start.AddHours(-6).Date.AddHours(6);
                end = end.AddHours(-6).Date.AddDays(1).AddHours(6);
                var result = await _repo.GetEstatusPedidos(start, end);
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
        public async Task<IActionResult> GetHistoricoVentas(DateTime start, DateTime end)
        {
            try
            {
                start = start.AddHours(-6).Date.AddHours(6);
                end = end.AddHours(-6).Date.AddDays(1).AddHours(6);
                var result = await _repo.GetHistoricoVentas(start, end);
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
