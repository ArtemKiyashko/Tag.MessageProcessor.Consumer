using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

internal interface IChatTitleRepository
{
    Task SetAlternativePrompt(long chatId, string title, string prompt);
    Task<ChatTitleEntity?> GetChatTitle(string title, long chatId);
}
