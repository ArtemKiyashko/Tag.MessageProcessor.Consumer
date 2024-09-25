using System;

namespace Tag.MessageProcessor.Repositories;

internal interface IChatTitleRepository
{
    Task SetAlternativePrompt(long chatId, string title, string prompt);
}
