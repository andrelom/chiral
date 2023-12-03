using Chiral.Protocol;

namespace Chiral.Network.Transport;

public class Peer : Node, IPeer
{
    public Peer(string host, int port) : base(host, port)  { }
}
