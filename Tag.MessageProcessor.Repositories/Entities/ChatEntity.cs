namespace Tag.MessageProcessor.Repositories.Entities;

public class ChatEntity : BaseEntity
{
    public long ChatTgId { get; set; }
    public required string Title { get; set; }
    
    public DateTimeOffset CreatedDateTimeUtc { get; set; } = DateTimeOffset.UtcNow;
}
