namespace Tag.MessageProcessor.Consumer.Helpers;

public record TgCommand(
    TgCommandTypes CommandType, 
    string? TgCommandText, 
    string? TgCommandArguments);
