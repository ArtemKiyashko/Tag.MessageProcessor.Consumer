namespace Tag.MessageProcessor.Managers.Dtos;

public class GenerateActionDto
{
    public Guid ActionId { get; set; }
    public long ChatId { get; set; }
    public required string ChatTitle { get; set; }
    public string? AlternativePrompt { get; set; }
}
