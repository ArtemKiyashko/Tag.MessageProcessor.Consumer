using System;
using Azure.Data.Tables;

namespace Tag.MessageProcessor.Repositories;

internal class ChatTitleRepository : IChatTitleRepository
{
    private readonly TableClient _tableClient;

    public ChatTitleRepository(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public Task SetAlternativePrompt(long chatId, string title, string prompt)
    {
        throw new NotImplementedException();
    }
}
