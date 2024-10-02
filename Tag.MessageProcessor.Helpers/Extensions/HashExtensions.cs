using System.Security.Cryptography;
using System.Text;

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
        var partitionKey = rowKey[..4];
        return (rowKey, partitionKey);
    }
}
