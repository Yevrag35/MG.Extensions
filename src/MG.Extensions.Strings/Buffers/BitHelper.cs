using MG.Extensions.Guarding;
using System.Numerics;

namespace MG.Extensions.Strings.Buffers
{
    /// <summary>
    /// A helper class for working with bits.
    /// </summary>
    internal static class BitHelper
    {
        /// <summary>
        /// The largest power of 2 that can be represented by an <see cref="int"/>.
        /// </summary>
        /// <value>
        /// <c>2^30</c> -or- <c>1,073,741,824</c>.
        /// </value>
        internal const uint LargestIntPowerOf2 = 1_073_741_824;
        private const int MINIMUM_POWER = 2;

        /// <summary>
        /// Rounds the given <see cref="int"/> value up to the next power of 2.
        /// </summary>
        /// <param name="value">The value to round up.</param>
        /// <returns>The next power of 2 greater than or equal to the specified value.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
        internal static int RoundUpToPowerOf2(int value)
        {
            Guard.ThrowIfNegativeOrGreaterThan(value, LargestIntPowerOf2);

            return value switch
            {
                < MINIMUM_POWER => MINIMUM_POWER,
                _ => (int)BitOperations.RoundUpToPowerOf2((uint)value),
            };
        }
        /// <summary>
        /// Rounds and sets the given <see cref="int"/> value up to the next power of 2.
        /// </summary>
        /// <remarks>
        /// This method does not check for negative or overflowable values.
        /// </remarks>
        /// <param name="value">The value to round up.</param>
        internal static int RoundUpToPowerOf2Unsafe(int value)
        {
            Debug.Assert(value > 0, "Value is 0 or negative!");
            return (int)BitOperations.RoundUpToPowerOf2((uint)value);
        }
    }
}
