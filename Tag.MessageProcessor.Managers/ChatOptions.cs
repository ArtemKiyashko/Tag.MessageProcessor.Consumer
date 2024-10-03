using System;

namespace Tag.MessageProcessor.Managers;

public class ChatOptions
{
    public Uri? TablesServiceUri { get; set; }
    public string? TablesConnectionString { get; set; }
    public string ChatTable { get; set; } = "tagchats";
    public string ChatTitleTable { get; set; } = "tagchattitles";

}
