using Tag.MessageProcessor.Consumer.ViewModels;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public interface ICommandHandler
{
    Task ProcessCommand(TgData tgData);
}
