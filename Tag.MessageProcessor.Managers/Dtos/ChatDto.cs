using System;

namespace Tag.MessageProcessor.Managers.Dtos;

public class ChatDto
{
    public required long ChatTgId { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public required string Title { get; set; }
    public string? AlternativePrompt { get; set; }
}
