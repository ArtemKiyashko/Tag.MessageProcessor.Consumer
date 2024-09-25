using System;
using Tag.MessageProcessor.Managers.Dtos;

namespace Tag.MessageProcessor.Managers;

public interface IChatManager
{
    Task UpsertChat(ChatDto chatDto);
}
