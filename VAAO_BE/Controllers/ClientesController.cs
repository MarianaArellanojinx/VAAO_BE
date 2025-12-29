


using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers

{


    [ApiController]
    [Route("api/[controller]/[action]")]

    public class ClientesController:ControllerBase
    {
        private readonly IClientesRepository _clientesRepository;
        public ClientesController(IClientesRepository clientesRepository)
        {
            _clientesRepository = clientesRepository;
        }

        [HttpPost]
        public async Task<IActionResult> InsertClientes(Clientes payload)
        {
            await _clientesRepository.CreateCliente(payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var result = await _clientesRepository.GetAllClientes();
            return Ok(new
            {
                data = result,
                message = string.Empty,
                status = true
            });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(int id, Clientes payload)
        {
            await _clientesRepository.UpdateCliente(id, payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCientes(int id)
        {
            await _clientesRepository.DeleteCliente(id);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

    }
}
