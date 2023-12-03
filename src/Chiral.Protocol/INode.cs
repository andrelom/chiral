namespace Chiral.Protocol;

public interface INode
{
    Key Key { get; }

    string Host { get; }

    int Port { get; }
}
