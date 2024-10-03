namespace Tag.MessageProcessor.Consumer.ViewModels;

public record TgData(
    TgCommand TgCommand,
    string ChatTitle,
    long ChatTgId);
