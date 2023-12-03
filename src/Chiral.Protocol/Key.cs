using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Chiral.Protocol.Exceptions;
using Chiral.Protocol.Extensions;

namespace Chiral.Protocol
{
    /// <summary>
    /// Represents a unique 160-bit identifier.
    /// </summary>
    public sealed class Key
    {
        private readonly BigInteger _data;

        private static readonly SHA1 Hash = SHA1.Create();

        public Key()
        {
            _data = BigInteger.Abs(new BigInteger(GenerateBytes()));
        }

        public Key(byte[] bytes)
        {
            if (!IsValid(bytes))
            {
                throw new InvalidKeyLengthException();
            }

            _data = new BigInteger(bytes);
        }

        /// <summary>
        /// Returns the distance between the current Key and the target Key.
        /// </summary>
        /// <param name="key">A Key.</param>
        /// <returns>The distance.</returns>
        public BigInteger DistanceTo(Key key)
        {
            // The BigInteger.Abs is used to ensure that the distance is a positive value, as
            // XOR can produce negative numbers when dealing with BigIntegers.
            return BigInteger.Abs(_data ^ key._data);
        }

        /// <summary>
        /// Converts a Key to a binary string.
        /// </summary>
        /// <returns>A string containing a binary representation of the Key.</returns>
        public string ToBinaryString()
        {
            return _data.ToBinaryString();
        }

        #region Operator Overloading (==, !=)

        public static bool operator ==(Key source, Key target)
        {
            return source?._data == target?._data;
        }

        public static bool operator !=(Key source, Key target)
        {
            return source?._data != target?._data;
        }

        public static bool operator ==(Key source, BigInteger target)
        {
            return source?._data == target;
        }

        public static bool operator !=(Key source, BigInteger target)
        {
            return source?._data != target;
        }

        public static bool operator ==(BigInteger source, Key target)
        {
            return source == target?._data;
        }

        public static bool operator !=(BigInteger source, Key target)
        {
            return source != target?._data;
        }

        #endregion

        #region Operator Overloading (<=, >=)

        public static bool operator <=(Key source, Key target)
        {
            return source._data <= target._data;
        }

        public static bool operator >=(Key source, Key target)
        {
            return source._data >= target._data;
        }

        public static bool operator <=(Key source, BigInteger target)
        {
            return source._data <= target;
        }

        public static bool operator >=(Key source, BigInteger target)
        {
            return source._data >= target;
        }

        public static bool operator <=(BigInteger source, Key target)
        {
            return source <= target._data;
        }

        public static bool operator >=(BigInteger source, Key target)
        {
            return source >= target._data;
        }

        #endregion

        #region Operator Overloading (<, >)

        public static bool operator <(Key source, Key target)
        {
            return source._data < target._data;
        }

        public static bool operator >(Key source, Key target)
        {
            return source._data > target._data;
        }

        public static bool operator <(Key source, BigInteger target)
        {
            return source._data < target;
        }

        public static bool operator >(Key source, BigInteger target)
        {
            return source._data > target;
        }

        public static bool operator <(BigInteger source, Key target)
        {
            return source < target._data;
        }

        public static bool operator >(BigInteger source, Key target)
        {
            return source > target._data;
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return HashCode.Combine(_data);
        }

        public override bool Equals(object? target)
        {
            return target is Key key && _data.Equals(key._data);
        }

        public override string ToString()
        {
            return _data.ToString();
        }

        #endregion

        #region Private Methods

        private bool IsValid(byte[] bytes)
        {
            return bytes.Length == Kademlia.B;
        }

        private byte[] GenerateBytes()
        {
            return Hash.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
        }

        #endregion
    }
}
