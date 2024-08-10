using MG.Extensions.Strings.Enumerators;

namespace MG.Extensions.Strings
{
    public static class StringSpanSplitExtensions
    {
        /// <summary>
        /// Splits the characters in the span based on a single separator character.
        /// </summary>
        /// <param name="span">The span of characters to split.</param>
        /// <param name="splitBy">The character to use as a separator.</param>
        /// <returns>A <see cref="SplitEnumerator"/> for enumerating over the split entries.</returns>
        [DebuggerStepThrough]
        public static SplitEnumerator SpanSplit(this Span<char> span, in char splitBy)
        {
            return new SplitEnumerator(str: span, SplitEnumerator.FromOneChar(in splitBy));
        }

        /// <summary>
        /// Splits the characters in the read-only span based on a single separator character.
        /// </summary>
        /// <param name="value">The read-only span of characters to split.</param>
        /// <param name="splitBy">The character to use as a separator.</param>
        /// <returns>A <see cref="SplitEnumerator"/> for enumerating over the split entries.</returns>
        [DebuggerStepThrough]
        public static SplitEnumerator SpanSplit(this ReadOnlySpan<char> value, in char splitBy)
        {
            return new SplitEnumerator(value, SplitEnumerator.FromOneChar(in splitBy));
        }
        /// <summary>
        /// Splits the characters in the read-only span based on a sequence of separator characters.
        /// </summary>
        /// <param name="readOnlyValue">The read-only span of characters to split.</param>
        /// <param name="splitBy">The span of characters to use as separators.</param>
        /// <returns>A <see cref="SplitEnumerator"/> for enumerating over the split entries.</returns>
        [DebuggerStepThrough]
        public static SplitEnumerator SpanSplit(this ReadOnlySpan<char> readOnlyValue, ReadOnlySpan<char> splitBy)
        {
            return new SplitEnumerator(readOnlyValue, splitBy);
        }
        /// <summary>
        /// Splits the string based on a single separator character.
        /// </summary>
        /// <param name="value">
        ///     The string to split. A <see langword="null"/> string is treated as an 
        ///     empty span.
        /// </param>
        /// <param name="splitBy">The character to use as a separator.</param>
        /// <returns>A <see cref="SplitEnumerator"/> for enumerating over the split entries.</returns>
        [DebuggerStepThrough]
        public static SplitEnumerator SpanSplit(this string? value, in char splitBy)
        {
            return new SplitEnumerator(value.AsSpan(), SplitEnumerator.FromOneChar(in splitBy));
        }
        /// <summary>
        /// Splits the string based on a sequence of separator characters.
        /// </summary>
        /// <param name="value">
        ///     The string to split. A <see langword="null"/> string is treated as an 
        ///     empty span.
        /// </param>
        /// <param name="splitBy">The span of characters to use as separators.</param>
        /// <returns>A <see cref="SplitEnumerator"/> for enumerating over the split entries.</returns>
        [DebuggerStepThrough]
        public static SplitEnumerator SpanSplit(this string? value, ReadOnlySpan<char> splitBy)
        {
            return new SplitEnumerator(value.AsSpan(), splitBy);
        }
        /// <summary>
        /// Splits the string based on a sequence of separator characters.
        /// </summary>
        /// <param name="value">
        ///     The string to split. A <see langword="null"/> string is treated as an 
        ///     empty span.
        /// </param>
        /// <param name="splitBy">The span of characters to use as separators.</param>
        /// <returns>A <see cref="SplitEnumerator"/> for enumerating over the split entries.</returns>
        [DebuggerStepThrough]
        public static SplitEnumerator SpanSplit(this Span<char> span, ReadOnlySpan<char> splitBy)
        {
            return new SplitEnumerator(str: span, splitBy);
        }

        //public static SplitAnyEnumerator SpanSplitAny(this ReadOnlySpan<char> chars, ReadOnlySpan<char> splitByAny)
        //{
        //    return new SplitAnyEnumerator(chars, splitByAny);
        //}
        //public static SplitAnyEnumerator SpanSplitAny(this string? value, ReadOnlySpan<char> splitByAny)
        //{
        //    return new SplitAnyEnumerator(value.AsSpan(), splitByAny);
        //}
    }
}
