namespace Chiral.Protocol
{
    /// <summary>
    /// The routing table is a binary tree whose leaves are k-buckets. The structure of
    /// the Kademlia routing table is such that nodes maintain detailed knowledge of
    /// the address space closest to them, and exponentially decreasing knowledge of
    /// more distant address space.
    /// </summary>
    /// <typeparam name="T">A Node.</typeparam>
    public interface IRoutingTable<T> where T : INode
    {
        /// <summary>
        /// Will add a new Node.
        /// </summary>
        /// <param name="node">A Node.</param>
        /// <returns>If the Node was added.</returns>
        public bool AddNode(T node);

        /// <summary>
        /// Will remove a specific Node.
        /// </summary>
        /// <param name="node">A Node.</param>
        /// <returns>If the Node was removed.</returns>
        public bool RemoveNode(T node);

        /// <summary>
        /// Will search for a specific Node using its Key value.
        /// </summary>
        /// <param name="key">A Key.</param>
        /// <returns>A Node.</returns>
        public T? FindNodeByKey(Key key);

        /// <summary>
        /// Returns the Nodes closest to the Key.
        /// </summary>
        /// <param name="key">A Key.</param>
        /// <returns>The Nodes.</returns>
        public IEnumerable<T> FindNearbyNodesByKey(Key key);
    }
}
