namespace Chiral.Network.Transport;

public interface IMessage
{
    string Type { get; set; }

    string Body { get; set; }
}
