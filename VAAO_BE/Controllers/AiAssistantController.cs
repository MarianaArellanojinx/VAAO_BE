using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Services;

namespace VAAO_BE.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AiAssistantController : ControllerBase
{
    private readonly ISalesAiAssistantService _salesAiAssistantService;

    public AiAssistantController(ISalesAiAssistantService salesAiAssistantService)
    {
        _salesAiAssistantService = salesAiAssistantService;
    }

    [HttpPost]
    public async Task<IActionResult> ChatVentas(AppAiChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                data = false,
                message = "El campo message es obligatorio.",
                status = false
            });
        }

        try
        {
            var result = await _salesAiAssistantService.ChatAsync(request, cancellationToken);
            return Ok(new
            {
                data = result,
                message = string.Empty,
                status = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                data = false,
                message = ex.Message,
                status = false
            });
        }
    }
}
