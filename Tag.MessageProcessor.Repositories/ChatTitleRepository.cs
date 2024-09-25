using System;

namespace Tag.MessageProcessor.Repositories;

internal class ChatTitleRepository : IChatTitleRepository
{
    public Task SetAlternativePrompt(long chatId, string title, string prompt)
    {
        throw new NotImplementedException();
    }
}
