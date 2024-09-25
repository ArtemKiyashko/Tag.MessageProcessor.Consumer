using System;

namespace Tag.MessageProcessor.Managers;

public class ChatOptions
{
    public Uri? ServiceUri { get; set; }
    public string? TableConnectionString { get; set; }
    public string ChatTable { get; set; } = "chats";
    public string ChatTitleTable { get; set; } = "chattitles";

}
