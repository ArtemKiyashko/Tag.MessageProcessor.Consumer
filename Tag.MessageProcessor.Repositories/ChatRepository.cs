using Azure.Data.Tables;
using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

internal class ChatRepository : IChatRepository
{
    private readonly TableClient _tableClient;

    public ChatRepository(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public Task<ChatEntity> GetChatById(long id)
    {
        throw new NotImplementedException();
    }

    public Task UpsertChat(ChatEntity chatEntity)
    {
        throw new NotImplementedException();
    }
}
