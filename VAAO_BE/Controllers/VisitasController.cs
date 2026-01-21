using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class VisitasController : ControllerBase
    {
        private readonly IVisitasRepository _repo;

        public VisitasController(IVisitasRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetVisitas(DateTime date)
        {
            date = date.AddHours(-6);
            var result = await _repo.GetVisitas(date);
            return Ok(new
            {
                data = result,
                message = "",
                status = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> InsertVisita(Visitas payload)
        {
            await _repo.InsertVisita(payload);
            return Ok(new
            {
                data = true,
                message = "",
                status = true
            });
        }
    }
}
