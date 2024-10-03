using Tag.MessageProcessor.Managers.Dtos;
using Tag.MessageProcessor.Repositories;
using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Managers;

internal class GenerateRequestManager(IChatManager chatManager, IGenerateRequestRepository generateRequestRepository) : IGenerateRequestManager
{
    private readonly IChatManager _chatManager = chatManager;
    private readonly IGenerateRequestRepository _generateRequestRepository = generateRequestRepository;

    public async Task<ChatDto?> EnqueueGenerateRequest(long chatId)
    {
        var chatDto = await _chatManager.GetChatById(chatId);
        if (chatDto is null)
            return default;
        
        var requestEntity = new GenerateRequestEntity{
            RequestType = GenerateRequestTypes.Enqueue,
            ChatTgId = chatDto.ChatTgId,
            ChatTitle = chatDto.Title,
            AlternativePrompt = chatDto.AlternativePrompt
        };

        await _generateRequestRepository.SendRequest(requestEntity);

        return chatDto;
    }
}
