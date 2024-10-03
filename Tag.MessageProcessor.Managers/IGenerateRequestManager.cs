using Tag.MessageProcessor.Managers.Dtos;

namespace Tag.MessageProcessor.Managers;

public interface IGenerateRequestManager
{
    Task<ChatDto?> EnqueueGenerateRequest(long chatId);
}
