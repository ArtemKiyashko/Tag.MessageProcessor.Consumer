using Tag.MessageProcessor.Consumer.CommandHandlers;
using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;

namespace Tag.MessageProcessor.Consumer.CommandDecorators;

public class TitleCheckDecorator(ICommandHandler commandHandler, IChatManager chatManager) : ICommandHandler
{
    private readonly ICommandHandler _commandHandler = commandHandler;
    private readonly IChatManager _chatManager = chatManager;

    public async Task ProcessCommand(TgData tgData)
    {
        var currentChat = await _chatManager.GetChatById(tgData.ChatTgId);
        if (currentChat is not null && !currentChat.Title.Equals(tgData.ChatTitle))
        {
            currentChat.Title = tgData.ChatTitle;
            await _chatManager.UpdateChatTitle(currentChat);
        }

        await _commandHandler.ProcessCommand(tgData);
    }
}
