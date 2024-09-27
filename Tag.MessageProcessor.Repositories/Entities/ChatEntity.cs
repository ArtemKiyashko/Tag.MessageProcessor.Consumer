namespace Tag.MessageProcessor.Repositories.Entities;

public class ChatEntity : BaseEntity
{
    public long ChatId { get; set; }
    public required string Title { get; set; }
}
