#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Enumerators
{
    internal static class SplitSearchValuesEntry
    {
        private static readonly Lazy<SearchValues<char>> s_lazySearchChars = new(() => SearchValues.Create(ReadOnlySpan<char>.Empty));
        internal static SearchValues<char> GetEmpty() => s_lazySearchChars.Value;

        public static SplitSearchValuesEntry<char> EmptyCharEntry => new([], GetEmpty());

#if NET9_0_OR_GREATER
        private static readonly Lazy<SearchValues<string>> s_lazySearchStrs = new(() => SearchValues.Create([], StringComparison.Ordinal));

        internal static SearchValues<string> GetEmptyStrings() => s_lazySearchStrs.Value;
        public static SplitSearchValuesEntry<string> EmptyStringEntry => new([], GetEmptyStrings());
#endif
    }

    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{{Section}\}")]
    public readonly ref struct SplitSearchValuesEntry<T> where T : IEquatable<T>
    {
        public readonly ReadOnlySpan<char> Section;
        public readonly SearchValues<T> SplitBy { get; }

        internal SplitSearchValuesEntry(ReadOnlySpan<char> section, SearchValues<T> searchValues)
        {
            Section = section;
            this.SplitBy = searchValues;
        }

        /// <summary>
        /// Implicitly converts a <see cref="SplitSearchValuesEntry"/> to a <see cref="ReadOnlySpan{T}"/>, returning
        /// <see cref="Section"/>.
        /// </summary>
        /// <param name="entry"></param>
        public static implicit operator ReadOnlySpan<char>(SplitSearchValuesEntry<T> entry)
        {
            return entry.Section;
        }
    }
}
#endif