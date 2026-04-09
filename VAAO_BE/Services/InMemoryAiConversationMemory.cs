using System.Collections.Concurrent;
using VAAO_BE.Entities;

namespace VAAO_BE.Services;

public class InMemoryAiConversationMemory : IAiConversationMemory
{
    private readonly ConcurrentDictionary<string, List<OpenAiChatMessage>> _conversations = new();

    public IReadOnlyList<OpenAiChatMessage> GetHistory(string conversationId, int maxMessages = 8)
    {
        if (!_conversations.TryGetValue(conversationId, out var messages))
        {
            return Array.Empty<OpenAiChatMessage>();
        }

        lock (messages)
        {
            return messages.TakeLast(maxMessages).ToList();
        }
    }

    public void AddMessage(string conversationId, string role, string content)
    {
        var messages = _conversations.GetOrAdd(conversationId, _ => new List<OpenAiChatMessage>());

        lock (messages)
        {
            messages.Add(new OpenAiChatMessage
            {
                Role = role,
                Content = content
            });

            if (messages.Count > 20)
            {
                messages.RemoveRange(0, messages.Count - 20);
            }
        }
    }
}
