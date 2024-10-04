using Azure.Data.Tables;
using Tag.MessageProcessor.Helpers.Extensions;
using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

internal class ChatTitleRepository(TableClient tableClient) : IChatTitleRepository
{
    private readonly TableClient _tableClient = tableClient;

    public async Task DeleteChatTitle(long chatId, string title)
    {
        var existingChatTitle = await GetChatTitle(chatId, title);
        if (existingChatTitle is null)
            return;
        existingChatTitle.IsDeleted = true;

        await _tableClient.UpdateEntityAsync(existingChatTitle, existingChatTitle.ETag, TableUpdateMode.Merge);
    }

    public async Task<ChatTitleEntity?> GetChatTitle(long chatId, string title)
    {
        var (rowKey, partitionKey) = HashExtensions.GetEntityKeyData(title, chatId);
        var existingChatTitle = await _tableClient.GetEntityIfExistsAsync<ChatTitleEntity>(partitionKey, rowKey);
        return existingChatTitle.HasValue && !existingChatTitle.Value.Disabled ? existingChatTitle.Value : default;
    }

    public async Task<IEnumerable<ChatTitleEntity>> GetChatTitles(long chatId)
    {
        var result = new List<ChatTitleEntity>();
        var existingChatTitles = _tableClient.QueryAsync<ChatTitleEntity>(ct => ct.PartitionKey == chatId.ToString() && ct.ChatId == chatId);
        
        await foreach(var chatTitleEntity in existingChatTitles)
            if (!chatTitleEntity.Disabled)
                result.Add(chatTitleEntity);

        return result;
    }

    public async Task SetAlternativePrompt(long chatId, string title, string prompt)
    {
        var (rowKey, partitionKey) = HashExtensions.GetEntityKeyData(title, chatId);
        var chatTitleEntity = new ChatTitleEntity
        {
            RowKey = rowKey,
            PartitionKey = partitionKey,
            Title = title,
            AlternativePrompt = prompt,
            ChatId = chatId
        };

        await _tableClient.UpsertEntityAsync(chatTitleEntity, TableUpdateMode.Merge);
    }
}
