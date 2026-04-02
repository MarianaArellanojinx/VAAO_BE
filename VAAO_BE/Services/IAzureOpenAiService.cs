using VAAO_BE.Entities;

namespace VAAO_BE.Services;

public interface IAzureOpenAiService
{
    Task<OpenAiChatResponse> CreateChatCompletionAsync(OpenAiChatRequest request, CancellationToken cancellationToken = default);
}
