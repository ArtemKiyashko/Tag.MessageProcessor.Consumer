namespace Tag.MessageProcessor.Consumer.ViewModels;

public record TgCommand(
    TgCommandTypes CommandType,
    string? TgCommandText,
    string? TgCommandArguments);
