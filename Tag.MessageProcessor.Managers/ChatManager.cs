using System.Security.Cryptography;
using Tag.MessageProcessor.Helpers.Extensions;
using Tag.MessageProcessor.Managers.Dtos;
using Tag.MessageProcessor.Managers.Extensions;
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
            await EnableChat(existingChat);
            return;
        }

        var keyData = HashExtensions.GetEntityKeyData(chatDto.ChatTgId);
        var newChat = new ChatEntity
        {
            RowKey = keyData.rowKey,
            PartitionKey = keyData.partitionKey,
            Title = chatDto.Title,
            IsDeleted = chatDto.IsDeleted,
        };

        await _chatRepository.InsertChat(newChat);
    }

    private async Task EnableChat(ChatEntity existingChat)
    {
        existingChat.IsDeleted = false;
        await _chatRepository.UpdateChat(existingChat);
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
}
