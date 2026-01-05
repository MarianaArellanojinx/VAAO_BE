

using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{


    [ApiController]
    [Route("api/[controller]/[action]")]

    public class MetodosPagoController:ControllerBase
    {

        private readonly IMetodosPago _metodospagoRepository;


        public MetodosPagoController(
            IMetodosPago metodospagoRepository)
        {
            _metodospagoRepository = metodospagoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMetodosPago()
        {
            var result = await _metodospagoRepository.GetAllMetodosPago();
            return Ok(new
            {
                data = result,
                message = string.Empty,
                status = true
            });
        }
    }
}
