namespace Tag.MessageProcessor.Managers;

public class GenerateRequestOptions
{
    public string TopicName { get; set; } = "generaterequests";
    public string? ServiceBusNamespace { get; set; }
    public string? ServiceBusConnectionString { get; set; }
}
