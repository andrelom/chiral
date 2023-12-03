using System.Numerics;
using Chiral.Protocol.Extensions;

namespace Chiral.Protocol
{
    public class Bucket<T> : IBucket<T> where T : INode
    {
        private readonly BigInteger _lower;

        private readonly BigInteger _upper;

        private readonly IList<T> _nodes;

        private readonly IList<T> _replacements;

        public Bucket()
        {
            _lower = 0;
            _upper = BigInteger.Pow(new BigInteger(2), Kademlia.B * 8);
            _nodes = new List<T>();
            _replacements = new List<T>();
        }

        private Bucket(BigInteger lower, BigInteger upper)
        {
            _lower = lower;
            _upper = upper;
            _nodes = new List<T>();
            _replacements = new List<T>();
        }

        public int Depth()
        {
            // TODO: Improve method performance.

            var values = _nodes.Select(node => node.Key.ToBinaryString()).ToArray();
            var shared = values.FindSharedPrefix();

            return shared.Length;
        }

        public bool Fits(Key key)
        {
            return key < _upper;
        }

        public bool FitsInRange(Key key)
        {
            return key >= _lower && _upper >= key;
        }

        public bool Add(T node)
        {
            // If the node already exists in the recipient’s k-bucket, the recipient
            // moves it to the tail of the list, per section 2.2 of the paper.
            if (MoveToTailIfExists(_nodes, node))
            {
                return true;
            }

            // If the bucket is full, keep track of node in a replacement list,
            // per section 4.1 of the paper.
            if (IsFull(_nodes))
            {
                AddToReplacements(node);

                return false;
            }

            _nodes.Add(node);

            return true;
        }

        public bool Remove(T node)
        {
            var removed = _nodes.Remove(node);

            if (removed)
            {
                AddFromReplacements();
            }

            return _replacements.Remove(node) || removed;
        }

        public T? FindNodeByKey(Key key)
        {
            return FindNodeByKey(_nodes, key);
        }

        public (IBucket<T>, IBucket<T>) Split()
        {
            var nodes = _nodes.Concat(_replacements);
            var middle = (_lower + _upper) / 2;
            var lowerBucket = new Bucket<T>(_lower, middle);
            var upperBucket = new Bucket<T>(middle + 1, _upper);

            foreach (var node in nodes)
            {
                if (node.Key <= middle)
                {
                    lowerBucket.Add(node);
                }
                else
                {
                    upperBucket.Add(node);
                }
            }

            return (lowerBucket, upperBucket);
        }

        public IEnumerable<T> ToEnumerable()
        {
            return _nodes.ToArray();
        }

        #region Overrides

        public override int GetHashCode()
        {
            return HashCode.Combine(_lower, _upper);
        }

        public override bool Equals(object? target)
        {
            return target is Bucket<T> bucket &&
                   _lower.Equals(bucket._lower) &&
                   _upper.Equals(bucket._upper);
        }

        #endregion

        #region Private Methods

        private bool IsFull(ICollection<T> nodes)
        {
            return Kademlia.K <= nodes.Count;
        }

        private bool MoveToTailIfExists(IList<T> nodes, T node)
        {
            if (FindNodeByKey(nodes, node.Key) == null)
            {
                return false;
            }

            nodes.Remove(node);
            nodes.Add(node);

            return true;
        }

        private T? FindNodeByKey(IEnumerable<T> nodes, Key key)
        {
            return nodes.SingleOrDefault(target => target.Key.Equals(key));
        }

        private void AddToReplacements(T node)
        {
            if (MoveToTailIfExists(_replacements, node))
            {
                return;
            }

            if (IsFull(_replacements))
            {
                _replacements.RemoveAt(0);
            }

            _replacements.Add(node);
        }

        private void AddFromReplacements()
        {
            if (!_replacements.Any())
            {
                return;
            }

            var node = _replacements.Last();

            _replacements.Remove(node);
            _nodes.Add(node);
        }

        #endregion
    }
}
