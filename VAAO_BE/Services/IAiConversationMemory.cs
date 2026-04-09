using VAAO_BE.Entities;

namespace VAAO_BE.Services;

public interface IAiConversationMemory
{
    IReadOnlyList<OpenAiChatMessage> GetHistory(string conversationId, int maxMessages = 8);
    void AddMessage(string conversationId, string role, string content);
}
