using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Tag.MessageProcessor.Consumer;

public class ChatTitleWatcher(
    IChatManager chatManager,
    ICommandHandlerFactory commandHandlerFactory,
    ITelegramBotClient telegramBotClient,
    ILogger<ChatTitleWatcher> logger)
{
    private readonly IChatManager _chatManager = chatManager;
    private readonly ICommandHandlerFactory _commandHandlerFactory = commandHandlerFactory;
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;
    private readonly ILogger<ChatTitleWatcher> _logger = logger;

    [Function("TitleChecker")]
    public async Task Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer)
    {
        var chats = await _chatManager.GetAllChats();
        foreach (var chat in chats)
        {
            try
            {
                var currentChat = await _telegramBotClient.GetChatAsync(chat.ChatTgId);
                if (currentChat is not null && !chat.Title.Equals(currentChat.Title))
                {
                    ArgumentNullException.ThrowIfNullOrEmpty(currentChat.Title);

                    var newTitleCommand = _commandHandlerFactory.GetCommandHandler(TgCommandTypes.NewTitle);
                    ArgumentNullException.ThrowIfNull(newTitleCommand);
                    
                    var tgData = new TgData(
                        new TgCommand(TgCommandTypes.NewChat, null, currentChat.Title),
                        currentChat.Title,
                        currentChat.Id
                    );
                    await newTitleCommand.ProcessCommand(tgData);
                }
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 403)
            {
                await _chatManager.DeleteChat(chat.ChatTgId);
                _logger.LogInformation(ex, "Bot has been kicked from the chat {chatId}", chat.ChatTgId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception suppressed. ChatId: {chatId}", chat.ChatTgId);
            }
        }
    }
}
