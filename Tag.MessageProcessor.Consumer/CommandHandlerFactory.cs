using Tag.MessageProcessor.Consumer.CommandDecorators;
using Tag.MessageProcessor.Consumer.CommandHandlers;
using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer;

public class CommandHandlerFactory(IChatManager chatManager, IGenerateRequestManager generateRequestManager, ITelegramBotClient telegramBotClient) : ICommandHandlerFactory
{
    private readonly IChatManager _chatManager = chatManager;
    private readonly IGenerateRequestManager _generateRequestManager = generateRequestManager;
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

    public ICommandHandler? GetCommandHandler(TgCommandTypes commandType) => commandType switch {
        TgCommandTypes.NewChat => new NewChatCommand(_chatManager, _generateRequestManager, _telegramBotClient),
        TgCommandTypes.NewTitle => new NewTitleCommand(_chatManager, _generateRequestManager, _telegramBotClient),
        TgCommandTypes.CustomPrompt => new TitleCheckDecorator(new CustomPromptCommand(_chatManager, _generateRequestManager, _telegramBotClient), _chatManager),
        TgCommandTypes.PrivateChat => new PrivateChatCommand(_telegramBotClient),
        TgCommandTypes.GenerateAvatar => new TitleCheckDecorator(new GenerateCommand(_generateRequestManager, _telegramBotClient), _chatManager),
        TgCommandTypes.RemoveChat => new RemoveChatCommand(_chatManager),
        TgCommandTypes.ChatMigrate => new MigrateChatCommand(_chatManager),
        _ => default
    };
}
