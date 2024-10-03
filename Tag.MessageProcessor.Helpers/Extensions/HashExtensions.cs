using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

namespace Tag.MessageProcessor.Helpers.Extensions;

public static class HashExtensions
{
    public static string GetHexString(this byte[] source)
    {
        StringBuilder Sb = new();
        foreach (byte b in source)
            Sb.Append(b.ToString("x2"));
        return Sb.ToString();
    }

    public static (string rowKey, string partitionKey) GetEntityKeyData(long id)
    {
        var rowKeyByteArray = BitConverter.GetBytes(id);
        var rowKey = SHA256.HashData(rowKeyByteArray).GetHexString();
        var partitionKey = rowKey[^4..];
        return (rowKey, partitionKey);
    }

    public static (string rowKey, string partitionKey) GetEntityKeyData(string id, object partitionKey)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(partitionKey);

        var rowKeyByteArray = Encoding.UTF8.GetBytes(id);
        var rowKey = SHA256.HashData(rowKeyByteArray).GetHexString();
        return (rowKey, partitionKey.ToString());
    }
}
