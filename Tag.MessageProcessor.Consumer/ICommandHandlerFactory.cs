using Tag.MessageProcessor.Consumer.CommandHandlers;
using Tag.MessageProcessor.Consumer.ViewModels;

namespace Tag.MessageProcessor.Consumer;

public interface ICommandHandlerFactory
{
    ICommandHandler? GetCommandHandler(TgCommandTypes commandType);
}
