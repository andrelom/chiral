namespace Chiral.Protocol
{
    public class RoutingTable<T> : IRoutingTable<T> where T : INode
    {
        private readonly T _root;

        private readonly IList<IBucket<T>> _buckets;

        public RoutingTable(T root)
        {
            _root = root;
            _buckets = new List<IBucket<T>> { new Bucket<T>() };
        }

        public bool AddNode(T node)
        {
            var bucket = GetBucketFor(node.Key);

            // This will succeed unless the bucket is full.
            if (bucket.Add(node))
            {
                return true;
            }

            // Per section 4.2 of paper, split if the bucket has the own node in its range or
            // if the depth is not congruent to 0 mod 5.
            if (bucket.FitsInRange(_root.Key) || bucket.Depth() % 5 != 0)
            {
                return SplitAndAddNode(bucket, node);
            }

            throw new Exception("TODO: Section 4.1 of the Kademlia paper!");
        }

        public bool RemoveNode(T node)
        {
            return GetBucketFor(node.Key).Remove(node);
        }

        public T? FindNodeByKey(Key key)
        {
            return GetBucketFor(key).FindNodeByKey(key);
        }

        public IEnumerable<T> FindNearbyNodesByKey(Key key)
        {
            return GetBucketFor(key).ToEnumerable().OrderBy(node => node.Key.DistanceTo(key));
        }

        #region Private Methods

        private IBucket<T> GetBucketFor(Key key)
        {
            return _buckets.FirstOrDefault(bucket => bucket.Fits(key)) ?? throw new InvalidOperationException();
        }

        private bool SplitAndAddNode(IBucket<T> bucket, T node)
        {
            var lowerIndex = _buckets.IndexOf(bucket);
            var upperIndex = lowerIndex + 1;
            var (lowerBucket, upperBucket) = bucket.Split();

            _buckets.RemoveAt(lowerIndex);

            _buckets.Insert(lowerIndex, lowerBucket);
            _buckets.Insert(upperIndex, upperBucket);

            return AddNode(node);
        }

        #endregion
    }
}
