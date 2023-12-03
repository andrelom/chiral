namespace Chiral.Protocol
{
    /// <summary>
    /// Represents a contact on the Chiral Network.
    /// </summary>
    public abstract class Node : INode
    {
        protected Node(string host, int port)
        {
            Key = new Key();
            Host = host;
            Port = port;
        }

        protected Node(Key key, string host, int port)
        {
            Key = key;
            Host = host;
            Port = port;
        }

        /// <summary>
        /// The Node.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// The host name.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// The port number.
        /// </summary>
        public int Port { get; }

        #region Overrides

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        public override bool Equals(object? target)
        {
            return target is Node node && Key.Equals(node.Key);
        }

        #endregion
    }
}
