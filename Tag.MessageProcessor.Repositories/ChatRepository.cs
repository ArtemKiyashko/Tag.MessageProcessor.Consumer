using Azure.Data.Tables;
using Tag.MessageProcessor.Helpers.Extensions;
using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

internal class ChatRepository(TableClient tableClient) : IChatRepository
{
    private readonly TableClient _tableClient = tableClient;

    public async Task<ChatEntity?> GetChatById(long id)
    {
        var (rowKey, partitionKey) = HashExtensions.GetEntityKeyData(id);
        var entity = await _tableClient.GetEntityIfExistsAsync<ChatEntity>(partitionKey, rowKey);
        return entity.HasValue ? entity.Value : default;
    }

    public Task InsertChat(ChatEntity chatEntity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateChat(ChatEntity chatEntity)
    {
        throw new NotImplementedException();
    }

    public Task UpsertChat(ChatEntity chatEntity)
    {
        throw new NotImplementedException();
    }
}
