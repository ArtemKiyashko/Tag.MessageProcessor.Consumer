using Tag.MessageProcessor.Helpers.Extensions;
using Tag.MessageProcessor.Managers.Dtos;
using Tag.MessageProcessor.Repositories;
using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Managers;

internal class ChatManager(IChatRepository chatRepository, IChatTitleRepository chatTitleRepository) : IChatManager
{
    private readonly IChatRepository _chatRepository = chatRepository;
    private readonly IChatTitleRepository _chatTitleRepository = chatTitleRepository;

    public async Task DeleteChat(long chatId)
    {
        var existingChat = await _chatRepository.GetChatById(chatId);
        if (existingChat is not null)
        {
            existingChat.IsDeleted = true;
            await _chatRepository.UpdateChat(existingChat);
        }
    }

    public async Task InsertChat(ChatDto chatDto)
    {
        var existingChat = await _chatRepository.GetChatById(chatDto.ChatTgId);
        if (existingChat is not null)
        {
            existingChat.Title = chatDto.Title;
            existingChat.IsDeleted = false;
            await _chatRepository.UpdateChat(existingChat);
            return;
        }

        var (rowKey, partitionKey) = HashExtensions.GetEntityKeyData(chatDto.ChatTgId);
        var newChat = new ChatEntity
        {
            RowKey = rowKey,
            PartitionKey = partitionKey,
            Title = chatDto.Title,
            IsDeleted = chatDto.IsDeleted,
            ChatTgId = chatDto.ChatTgId
        };

        await _chatRepository.InsertChat(newChat);
    }

    public async Task UpdateChatTitle(ChatDto chatDto)
    {
        var existingChat = await _chatRepository.GetChatById(chatDto.ChatTgId);
        if (existingChat is not null)
        {
            existingChat.Title = chatDto.Title;
            await _chatRepository.UpdateChat(existingChat);
        }
    }

    public Task SetCustomPromptToChat(ChatDto chatDto, string prompt) =>
        _chatTitleRepository.SetAlternativePrompt(chatDto.ChatTgId, chatDto.Title, prompt);

    public async Task<ChatDto?> GetChatById(long chatId)
    {
        var existingChat = await _chatRepository.GetChatById(chatId);
        if (existingChat is not null)
        {
            var result = new ChatDto
            {
                ChatTgId = existingChat.ChatTgId,
                Title = existingChat.Title,
                CreatedDate = existingChat.CreatedDateTimeUtc,
                IsDeleted = existingChat.IsDeleted
            };
            var chatTitle = await _chatTitleRepository.GetChatTitle(chatId, existingChat.Title);
            if (chatTitle is not null)
                result.AlternativePrompt = chatTitle.AlternativePrompt;

            return result;
        }
        else return default;
    }

    public async Task MigrateChat(long fromId, long toId)
    {
        var oldChatTitles = await _chatTitleRepository.GetChatTitles(fromId);
        var oldChat = await _chatRepository.GetChatById(fromId);

        if (oldChat is null)
            return;

        foreach (var chatTitle in oldChatTitles)
        {
            await _chatTitleRepository.SetAlternativePrompt(toId, chatTitle.Title, chatTitle.AlternativePrompt);
            await _chatTitleRepository.DeleteChatTitle(fromId, chatTitle.Title);
        }
        
        var newChat = new ChatDto{
            ChatTgId = toId,
            Title = oldChat.Title
        };

        await InsertChat(newChat);
        await DeleteChat(fromId);
    }
}
