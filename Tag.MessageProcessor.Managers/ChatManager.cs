using Tag.MessageProcessor.Managers.Dtos;
using Tag.MessageProcessor.Repositories;

namespace Tag.MessageProcessor.Managers;

internal class ChatManager(IChatRepository chatRepository, IChatTitleRepository chatTitleRepository) : IChatManager
{
    private readonly IChatRepository _chatRepository = chatRepository;
    private readonly IChatTitleRepository _chatTitleRepository = chatTitleRepository;

    public Task UpsertChat(ChatDto chatDto)
    {
        throw new NotImplementedException();
    }
}
