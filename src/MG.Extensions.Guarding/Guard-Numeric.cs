using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace MG.Extensions.Guarding
{
    public static partial class Guard
    {
        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if a value is negative.
        /// </summary>
        /// <param name="value">The integer value to check.</param>
        /// <param name="paramName">The name of the parameter that holds the value.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is less than zero.</exception>
        public static void ThrowIfNegative(int value,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
            string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#else
            if (value < 0)
            {
                paramName ??= nameof(value);
                throw new ArgumentOutOfRangeException(paramName, value, "The value must be greater than or equal to 0.");
            }
#endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if a value is greater than
        /// <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as less or equal to than <paramref name="other"/>.
        /// </param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/></param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="value"/> is greater than <paramref name="other"/>.
        /// </exception>
        public static void ThrowIfGreaterThan(int value, int other,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
            string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
#else
            if (value > other)
            {
                paramName ??= nameof(value);
                throw new ArgumentOutOfRangeException(paramName, value, $"The value must be less than or equal to {other}.");
            }
#endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if a value is greater than or equal 
        /// than <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as less than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/></param> 
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="value"/> is greater than or equal to <paramref name="other"/>.
        /// </exception>
        public static void ThrowIfGreaterThanOrEqual(int value, int other,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
    string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
#else
            if (value >= other)
            {
                paramName ??= nameof(value);
                throw new ArgumentOutOfRangeException(paramName, value, $"The value must be less than or equal to {other}.");
            }
#endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if a value is less than 
        /// <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as less or equal to than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/></param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="value"/> is less than <paramref name="other"/>.
        /// </exception>
        public static void ThrowIfLessThan(int value, int other,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
            string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
#else
            if (value < other)
            {
                paramName ??= nameof(value);
                throw new ArgumentOutOfRangeException(paramName, value, $"The value must be less than or equal to {other}.");
            }
#endif
        }
    }
}