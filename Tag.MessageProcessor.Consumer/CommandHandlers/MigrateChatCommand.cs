using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class MigrateChatCommand(IChatManager chatManager) : ICommandHandler
{
private readonly IChatManager _chatManager = chatManager;

    public Task ProcessCommand(TgData tgData) => _chatManager.MigrateChat(long.Parse(tgData.TgCommand.TgCommandArguments), tgData.ChatTgId);
}
