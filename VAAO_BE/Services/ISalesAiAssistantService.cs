using VAAO_BE.Entities;

namespace VAAO_BE.Services;

public interface ISalesAiAssistantService
{
    Task<AppAiChatResponse> ChatAsync(AppAiChatRequest request, CancellationToken cancellationToken = default);
}
