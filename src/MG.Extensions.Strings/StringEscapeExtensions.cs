namespace MG.Extensions.Strings
{
    /// <summary>
    /// Extensions methods for determining escape char sequences within <see cref="string"/> objects and spans.
    /// </summary>
    public static class StringEscapeExtensions
    {
        /// <summary>
        /// Determines if the character at the specified index is escaped with the specified escape character.
        /// </summary>
        /// <param name="readOnlySpanValue">The span of characters where the indexed character occurs.</param>
        /// <param name="index">
        ///     The index of the character within the span where the preceding characters will be checked.
        /// </param>
        /// <param name="escapeChar">
        ///     The character that is marked as the escape character in the span. Defaults to a backslash <c>\</c>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character at the specified index is found to be escaped;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsEscapedAt(this ReadOnlySpan<char> readOnlySpanValue, int index, char escapeChar = '\\')
        {
            int escapeCount = 0;
            // Count the number of escape characters preceding the current index.
            for (int i = index - 1; i >= 0 && escapeChar == readOnlySpanValue[i]; i--)
            {
                escapeCount++;
            }

            return 0 != escapeCount % 2;
        }

        /// <summary>
        /// Determines if the character at the specified index is escaped with the specified escape character.
        /// </summary>
        /// <param name="spanValue">The span of characters where the indexed character occurs.</param>
        /// <param name="index">
        ///     The index of the character within the span where the preceding characters will be checked.
        /// </param>
        /// <param name="escapeChar">
        ///     The character that is marked as the escape character in the span. Defaults to a backslash <c>\</c>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character at the specified index is found to be escaped;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsEscapedAt(this Span<char> spanValue, int index, char escapeChar = '\\')
        {
            return IsEscapedAt(readOnlySpanValue: spanValue, index, escapeChar);
        }

        /// <summary>
        /// Determines if the character at the specified index of this <see cref="string"/> is escaped with
        /// the specified escape character.
        /// </summary>
        /// <param name="value">The string of characters where the indexed character occurs.</param>
        /// <param name="index">
        ///     The index of the character within the span where the preceding characters will be check.
        /// </param>
        /// <param name="escapeChar">
        ///     The character that is marked as the escape character in the span.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character at the specified index is found to be escaped;
        /// otherwise, if it is not escaped or the <see cref="string"/> value is <see langword="null"/>, empty,
        /// or whitespace, <see langword="false"/>.
        /// </returns>
        public static bool IsEscapedAt(this string? value, int index, char escapeChar = '\\')
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return IsEscapedAt(readOnlySpanValue: value.AsSpan(), index, escapeChar);
        }
    }
}