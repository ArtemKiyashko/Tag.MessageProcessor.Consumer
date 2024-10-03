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
        return entity.HasValue && !entity.Value.Disabled ? entity.Value : default;
    }

    public Task InsertChat(ChatEntity chatEntity) => _tableClient.AddEntityAsync(chatEntity);

    public Task UpdateChat(ChatEntity chatEntity) => _tableClient.UpdateEntityAsync(chatEntity, chatEntity.ETag, TableUpdateMode.Merge);

    public Task UpsertChat(ChatEntity chatEntity) => _tableClient.UpsertEntityAsync(chatEntity, TableUpdateMode.Merge);
}
