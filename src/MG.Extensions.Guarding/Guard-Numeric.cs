using System;
using System.Diagnostics;
using System.Globalization;
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
                string format = !string.IsNullOrEmpty(paramName)
                    ? "{1} ('{0}') must be a non-negative value."
                    : "('{0}') must be a non-negative value.";

                throw new ArgumentOutOfRangeException(paramName, value, string.Format(
                    provider: CultureInfo.CurrentCulture,
                    format,
                    value,
                    paramName));
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
                string format = !string.IsNullOrEmpty(paramName)
                    ? "{2} ('{0}') must be less than or equal to '{1}'."
                    : "('{0}') must be less than or equal to '{1}'.";

                throw new ArgumentOutOfRangeException(paramName, value, string.Format(
                    provider: CultureInfo.CurrentCulture,
                    format,
                    value,
                    other,
                    paramName));
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
                string format = !string.IsNullOrEmpty(paramName)
                    ? "{2} ('{0}') must be less than '{1}'."
                    : "('{0}') must be less than '{1}'.";

                throw new ArgumentOutOfRangeException(paramName, string.Format(
                    provider: CultureInfo.CurrentCulture,
                    format,
                    value,
                    other,
                    paramName));
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
                string format = !string.IsNullOrEmpty(paramName)
                    ? "{2} ('{0}') must be greater than or equal to '{1}'."
                    : "('{0}') must be greater than or equal to '{1}'.";

                throw new ArgumentOutOfRangeException(paramName, string.Format(
                   provider: CultureInfo.CurrentCulture,
                   format,
                   value,
                   other,
                   paramName));
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
        public static void ThrowIfLessThanOrEqual(int value, int other,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
            string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
#else
            if (value < other)
            {
                string format = !string.IsNullOrEmpty(paramName)
                    ? "{2} ('{0}') must be greater than '{1}'."
                    : "('{0}') must be greater than '{1}'.";

                throw new ArgumentOutOfRangeException(paramName, string.Format(
                   provider: CultureInfo.CurrentCulture,
                   format,
                   value,
                   other,
                   paramName));
            }
#endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if the specified <paramref name="value"/> is negative  or
        /// greater than the specified <paramref name="other"/>.
        /// </summary>
        /// <remarks>This method is typically used to validate input parameters to ensure they fall within an acceptable
        /// range.</remarks>
        /// <param name="value">The integer value to validate. Must not be negative and must not exceed <paramref name="other"/>.</param>
        /// <param name="other">The upper limit, inclusive, that <paramref name="value"/> must not exceed. Must be less than or equal to <see
        /// cref="int.MaxValue"/>.</param>
        /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler if not explicitly
        /// provided.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative or greater than <paramref name="other"/>.</exception>
        public static void ThrowIfNegativeOrGreaterThan(int value, uint other,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
            string? paramName = null)
        {
            if ((uint)value > other)
            {
                string format = !string.IsNullOrEmpty(paramName)
                    ? "{2} ('{0}') must be a non-negative value greater than '{1}'."
                    : "('{0}') must be a non-negative value greater than '{1}'.";

                throw new ArgumentOutOfRangeException(paramName, string.Format(
                   provider: CultureInfo.CurrentCulture,
                   format,
                   value,
                   other,
                   paramName));
            }

            //u ('4294967292') must be less than or equal to '4'. (Parameter 'u')
            // Actual value was 4294967292.
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if the specified <paramref name="value"/> is negative  or
        /// greater than or equal to the specified <paramref name="other"/>.
        /// </summary>
        /// <remarks>This method is typically used to validate input parameters to ensure they fall within an acceptable
        /// range.</remarks>
        /// <param name="value">The integer value to validate.</param>
        /// <param name="other">The upper bound that <paramref name="value"/> must be less than. Must be less than or equal to <see
        /// cref="int.MaxValue"/>.</param>
        /// <param name="paramName">The name of the parameter being validated.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative or greater than or equal to <paramref name="other"/>.</exception>
        public static void ThrowIfNegativeOrGreaterThanOrEqual(int value, uint other,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
        string? paramName = null)
        {
            if ((uint)value >= other)
            {
                string format = !string.IsNullOrEmpty(paramName)
                    ? "{2} ('{0}') must be a non-negative value greater than or equal to '{1}'."
                    : "('{0}') must be a non-negative value greater than or equal to '{1}'.";

                throw new ArgumentOutOfRangeException(paramName, string.Format(
                   provider: CultureInfo.CurrentCulture,
                   format,
                   value,
                   other,
                   paramName));
            }
        }
        /// <summary>
        /// Throws an exception if the specified value is less than or equal to zero.
        /// </summary>
        /// <param name="value">The integer value to validate. Must be greater than zero.</param>
        /// <param name="paramName">The name of the parameter being validated. Used in the exception message if an error is thrown. Optional.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is less than or equal to zero.</exception>
        public static void ThrowIfNegativeOrZero(int value,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
        string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
#else
            if (value <= 0)
            {
                string format = !string.IsNullOrEmpty(paramName)
                    ? "{1} ('{0}') must be a non-negative and non-zero value."
                    : "('{0}') must be a non-negative and non-zero value.";

                throw new ArgumentOutOfRangeException(paramName, string.Format(
                   provider: CultureInfo.CurrentCulture,
                   format,
                   value,
                   paramName));
            }
#endif
        }
    }
}