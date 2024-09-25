using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

internal class ChatRepository : IChatRepository
{
    public Task<ChatEntity> GetChatById(long id)
    {
        throw new NotImplementedException();
    }

    public Task UpsertChat(ChatEntity chatEntity)
    {
        throw new NotImplementedException();
    }
}
