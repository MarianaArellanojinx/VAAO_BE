using Microsoft.AspNetCore.Mvc;
using VAAO_BE.Entities;
using VAAO_BE.Services;

namespace VAAO_BE.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AzureOpenAiController : ControllerBase
{
    private readonly IAzureOpenAiService _azureOpenAiService;

    public AzureOpenAiController(IAzureOpenAiService azureOpenAiService)
    {
        _azureOpenAiService = azureOpenAiService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat(OpenAiChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new
            {
                data = false,
                message = "El campo prompt es obligatorio.",
                status = false
            });
        }

        try
        {
            var result = await _azureOpenAiService.CreateChatCompletionAsync(request, cancellationToken);
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
