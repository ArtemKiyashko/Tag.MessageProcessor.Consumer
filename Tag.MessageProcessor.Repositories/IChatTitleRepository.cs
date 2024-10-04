using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

internal interface IChatTitleRepository
{
    Task SetAlternativePrompt(long chatId, string title, string prompt);
    Task<ChatTitleEntity?> GetChatTitle(long chatId, string title);
    Task<IEnumerable<ChatTitleEntity>> GetChatTitles(long chatId);
    Task DeleteChatTitle( long chatId, string title);
}
