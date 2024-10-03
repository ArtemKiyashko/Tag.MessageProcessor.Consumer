using Azure.Data.Tables;
using Tag.MessageProcessor.Helpers.Extensions;
using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

internal class ChatTitleRepository(TableClient tableClient) : IChatTitleRepository
{
    private readonly TableClient _tableClient = tableClient;

    public async Task<ChatTitleEntity?> GetChatTitle(string title, long chatId)
    {
        var (rowKey, partitionKey) = HashExtensions.GetEntityKeyData(title, chatId);
        var existingChatTitle = await _tableClient.GetEntityIfExistsAsync<ChatTitleEntity>(partitionKey, rowKey);
        return existingChatTitle.HasValue && !existingChatTitle.Value.Disabled ? existingChatTitle.Value : default;
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
