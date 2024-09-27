namespace Tag.MessageProcessor.Repositories.Entities;

public class ChatTitleEntity : BaseEntity
{
    public required string Title { get; set; }
    public long ChatId { get; set; }
    public required string AlternativePrompt { get; set; }
}
