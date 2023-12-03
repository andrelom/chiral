namespace Chiral.Protocol
{
    /// <summary>
    /// The Chiral Network (Kademlia) characterization.
    /// </summary>
    public static class Kademlia
    {
        /// <summary>
        /// The size in bytes of the Key used to identify a Node (160 bits).
        /// </summary>
        public const int B = 20;

        /// <summary>
        /// The maximum number of contacts stored in a Bucket.
        /// </summary>
        public const int K = 20;
    }
}
