using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ReportController: ControllerBase

    {
        private readonly IReportRepository _reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;   
        }

        [HttpGet]
        public async Task<IActionResult> GetVentasPerdidas()
        {
            var result = await  _reportRepository.ObtenerReporteVentaPerdida();
            return Ok(new
            {
                data = result,
                message = "",
                status = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPedidosRechazados()
        {
            var result = await _reportRepository.ObtenerReporteVentaRechazada();
            return Ok(new
            {
                data = result,
                message = "ok",
                status = true
            });
        }
    }
}
