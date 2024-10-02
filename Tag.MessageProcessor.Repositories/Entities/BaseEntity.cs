using Azure;
using Azure.Data.Tables;

namespace Tag.MessageProcessor.Repositories.Entities;

public class BaseEntity : ITableEntity
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool Disabled { get; set; } = false;
}
