using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace MG.Extensions.Strings
{
    /// <summary>
    /// Extension methods for <see cref="string"/> objects.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns either the specified string or an empty string if the specified string is <see langword="null"/>.
        /// </summary>
        /// <param name="value">The string to return if it is not <see langword="null"/>.</param>
        /// <returns>
        /// The specified string, <paramref name="value"/> if not <see langword="null"/>; otherwise, 
        /// <see cref="string.Empty"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static string OrEmpty(this string? value)
        {
            return value ?? string.Empty;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Count(ReadOnlySpan<char> value, ReadOnlySpan<char> span)
        {
#if NET8_0_OR_GREATER
            return value.Count(span);
#else
            switch (value.Length)
            {
                case 0:
                    return 0;

                case 1:
                    return Count(span, in value[0]);

                default:
                    int count = 0;

                    int pos;
                    while ((pos = span.IndexOf(value)) >= 0)
                    {
                        span = span.Slice(pos + value.Length);
                        count++;
                    }

                    return count;
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Count(ReadOnlySpan<char> value, in char c)
        {
#if NET8_0_OR_GREATER
            return value.Count(new ReadOnlySpan<char>(in c));
#else
            int count = 0;
            foreach (char v in value)
            {
                if (c == v)
                    count++;
            }

            return count;
#endif
        }
    }
}