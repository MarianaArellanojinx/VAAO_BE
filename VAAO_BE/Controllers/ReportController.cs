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
        public async Task<IActionResult> GetVentasPerdidas(DateTime start, DateTime end)
        {
            start = start.AddHours(-6);
            end = end.AddHours(-6);
            var result = await  _reportRepository.ObtenerReporteVentaPerdida(start, end);
            return Ok(new
            {
                data = result,
                message = "",
                status = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPedidosRechazados(DateTime startDate, DateTime endDate)
        {
            var start = startDate.AddHours(-6);
            var end = endDate.AddHours(-6);
            var result = await _reportRepository.ObtenerReporteVentaRechazada(start, end);
            return Ok(new
            {
                data = result,
                message = "ok",
                status = true
            });
        }
    }
}
