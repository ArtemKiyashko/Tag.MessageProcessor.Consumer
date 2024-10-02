using System;
using Tag.MessageProcessor.Managers.Dtos;

namespace Tag.MessageProcessor.Managers;

public interface IChatManager
{
    Task InsertChat(ChatDto chatDto);
    Task UpdateChatTitle(ChatDto chatDto);
    Task DeleteChat(long chatId);
}
