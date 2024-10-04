using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class RemoveChatCommand(IChatManager chatManager) : ICommandHandler
{
private readonly IChatManager _chatManager = chatManager;

    public Task ProcessCommand(TgData tgData) => _chatManager.DeleteChat(tgData.ChatTgId);
}
