namespace Chiral.Protocol
{
    /// <summary>
    /// A Kademlia Node organizes its contacts, other Nodes known to it, in Buckets
    /// which hold a maximum of K contacts. These are known as Buckets, and they are
    /// organized by the distance between the node and the contacts in the Bucket.
    /// </summary>
    /// <typeparam name="T">A Node.</typeparam>
    public interface IBucket<T> where T : INode
    {
        /// <summary>
        /// Returns the length of the longest common prefix shared between the Keys.
        /// </summary>
        /// <returns>The common prefix length.</returns>
        int Depth();

        /// <summary>
        /// Checks whether the supplied Key will fit within current Bucket range limit.
        /// </summary>
        /// <param name="key">A Key.</param>
        /// <returns>If the Key fits.</returns>
        bool Fits(Key key);

        /// <summary>
        /// Checks whether the supplied Key will fit within the current range.
        /// </summary>
        /// <param name="key">A Key.</param>
        /// <returns>If the Key fits.</returns>
        bool FitsInRange(Key key);

        /// <summary>
        /// Will add a new Node to the list, or move it to the end of the list if it
        /// exists, or send it to the replacement list.
        /// </summary>
        /// <param name="node">A Node.</param>
        /// <returns>Whether the Node was added or sent to the replacement list.</returns>
        bool Add(T node);

        /// <summary>
        /// Removes a specific Node and, if available, loads a new Node from the
        /// replacement list.
        /// </summary>
        /// <param name="node">A Node.</param>
        /// <returns>Whether the Node was found and removed.</returns>
        bool Remove(T node);

        /// <summary>
        /// Will search for a specific Node using its Key value.
        /// </summary>
        /// <param name="key">A Key.</param>
        /// <returns>A Node.</returns>
        T? FindNodeByKey(Key key);

        /// <summary>
        /// It will divide the Bucket in two, distributing the existing Nodes according to
        /// the range of each new Bucket.
        /// </summary>
        /// <returns>Two new Buckets.</returns>
        (IBucket<T>, IBucket<T>) Split();

        /// <summary>
        /// Converts the current Bucket to an IEnumerable.
        /// </summary>
        /// <returns>An array containing the Nodes.</returns>
        IEnumerable<T> ToEnumerable();
    }
}
