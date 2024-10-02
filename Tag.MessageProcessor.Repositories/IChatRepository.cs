using System.Runtime.CompilerServices;
using Tag.MessageProcessor.Repositories.Entities;

[assembly: InternalsVisibleTo("Tag.MessageProcessor.Managers")]
namespace Tag.MessageProcessor.Repositories;

internal interface IChatRepository
{
    Task<ChatEntity?> GetChatById(long id);
    Task UpsertChat(ChatEntity chatEntity);
    Task UpdateChat(ChatEntity chatEntity);
    Task InsertChat(ChatEntity chatEntity);
}
