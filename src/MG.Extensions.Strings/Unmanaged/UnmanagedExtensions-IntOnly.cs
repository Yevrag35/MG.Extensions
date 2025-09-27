#if !NET7_0_OR_GREATER

using System.Numerics;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Unmanaged
{
    /// <summary>
    /// Provides extension methods for unmanaged types.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Calculates the number of decimal digits required to represent the specified 32-bit signed integer, including
        /// the sign if negative.
        /// </summary>
        /// <remarks>This method provides an efficient way to determine the length of an integer's decimal
        /// representation without converting it to a string. For example, -123 returns 4 (three digits plus the
        /// negative sign), and 0 returns 1.</remarks>
        /// <param name="value">The integer value for which to determine the number of decimal digits. Negative values include the sign in
        /// the count.</param>
        /// <returns>The total number of characters needed to represent the integer in decimal form, including the negative sign
        /// if applicable.</returns>
        public static int GetLength(this int value)
        {
            if (value == 0 || value == 1)
            {
                return 1;
            }

            bool isNegative = value < 0;
            int negativeSign = Unsafe.As<bool, byte>(ref isNegative);
            int absValue = isNegative
                ? value == int.MinValue ? int.MaxValue : Math.Abs(value)
                : value;

            // log2(x) ≈ bitLength - 1;  Digits ≈ ceil(log10(x))
            // A very good integer approximation: digits = ((bitLen * 1233) >> 12) + 1
            //   1233 / 2^12  ≈ 0.30103  (log10(2))
            int bitLen = BitOperations.Log2((uint)absValue) + 1;
            int digits = (bitLen * 1233 >> 12) + 1;

            int lookup = GetPower(digits - 1);
            if (lookup > absValue)
                digits--;

            return digits + negativeSign;
        }

        /// <summary>
        /// Returns a reference to 10^<paramref name="index"/> for <see langword="int"/>.
        /// </summary>
        /// <remarks>
        /// Valid indexes are 0..9; 10^10 exceeds <see langword="int"/> (overflows).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPower(int index)
            => Unsafe.Add(ref MemoryMarshal.GetReference(s_pow10.AsSpan()), index);

        // 10^0 .. 10^9
        private static readonly int[] s_pow10 = new int[]
        {
            1,                // 10^0
            10,               // 10^1
            100,              // 10^2
            1_000,            // 10^3
            10_000,           // 10^4
            100_000,          // 10^5
            1_000_000,        // 10^6
            10_000_000,       // 10^7
            100_000_000,      // 10^8
            1_000_000_000     // 10^9
        };
    }
}
#endif