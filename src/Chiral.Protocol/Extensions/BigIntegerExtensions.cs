using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Chiral.Protocol.Extensions
{
    /// <summary>
    /// Extension methods for BigIntegers.
    /// </summary>
    internal static class BigIntegerExtensions
    {
        /// <summary>
        /// Converts a BigInteger to a binary string.
        /// </summary>
        /// <param name="value">A BigInteger.</param>
        /// <returns>A string containing a binary representation of the supplied BigInteger.</returns>
        public static string ToBinaryString(this BigInteger value)
        {
            var bytes = value.ToByteArray();
            var index = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var result = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[index], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && value.Sign == 1)
            {
                result.Append('0');
            }

            // Append binary string to String result.
            result.Append(binary);

            // Convert remaining bytes adding leading zeros.
            // Notice that each converted byte is padded on the left with zeros ('0'), as
            // necessary, so that the converted string is eight binary characters. This is
            // extremely important. Without this padding, the hexadecimal value '101' would be
            // converted to a binary value of '11'.
            for (index--; index >= 0; index--)
            {
                result.Append(Convert.ToString(bytes[index], 2).PadLeft(8, '0'));
            }

            // Because bytes consume full 8 bits, we might occasionally get leading zeros.
            return Regex.Replace(result.ToString(), @"^0+", "");
        }
    }
}
