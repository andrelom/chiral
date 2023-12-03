using System.Text.Json;

namespace Chiral.Network.Transport;

public class Message : IMessage
{
    public Message() { }

    public Message(string type, string body)
    {
        Type = type;
        Body = body;
    }

    public string Type { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public static async Task<Message?> DeserializeAsync(StreamReader reader, CancellationToken token)
    {
        var data = await reader.ReadToEndAsync(token);

        if (string.IsNullOrWhiteSpace(data))
        {
            return null;
        }

        return JsonSerializer.Deserialize<Message>(data.Trim());
    }
}
