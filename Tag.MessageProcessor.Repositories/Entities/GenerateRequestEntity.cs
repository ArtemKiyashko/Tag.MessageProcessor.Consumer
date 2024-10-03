using System;

namespace Tag.MessageProcessor.Repositories.Entities;

public class GenerateRequestEntity
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public required GenerateRequestTypes RequestType { get; set; }
    public required long ChatTgId { get; set; }
    public required string ChatTitle { get; set; }
    public string? AlternativePrompt { get; set; }
}
