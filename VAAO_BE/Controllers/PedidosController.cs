using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Repositories;
using VAAO_BE.Repositories.Interfaces;

namespace VAAO_BE.Controllers
{


    [ApiController]
    [Route("api/[controller]/[action]")]

    public class PedidosController:ControllerBase
    {

        private readonly IPedidoRepository _pedidoRepository;
        public PedidosController(IPedidoRepository pedidosRepository)
        {
            _pedidoRepository = pedidosRepository;
        }



        [HttpPost]
        public async Task<IActionResult> InsertPedidos(Pedidos payload)
        {
            await _pedidoRepository.CreatePedido(payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPedidos()
        {
            var result = await _pedidoRepository.GetAllPedidos();
            return Ok(new
            {
                data = result,
                message = string.Empty,
                status = true
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetPedidosFiltrados(DateTime start, DateTime end)
        {
            var result = await _pedidoRepository.GetAllPedidos();
            return Ok(new
            {
                data = result.Where(x => x.FechaPedido >= start && x.FechaPedido <= end).ToList(),
                message = string.Empty,
                status = true
            });
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePedido(int id, Pedidos payload)
        {
            await _pedidoRepository.UpdatePedido(id, payload);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedidos(int id)
        {
            await _pedidoRepository.DeletePedido(id);
            return Ok(new
            {
                data = true,
                message = string.Empty,
                status = true
            });
        }

    }
}
