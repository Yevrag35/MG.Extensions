using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Enumerators
{
    internal static class SplitSearchValuesEntry
    {
        private static readonly Lazy<SearchValues<char>> s_lazySearchChars = new(() => SearchValues.Create(ReadOnlySpan<char>.Empty));
        internal static SearchValues<char> GetEmpty() => s_lazySearchChars.Value;

        internal static SplitSearchValuesEntry<char> EmptyCharEntry => new(Range.All, GetEmpty());

#if NET9_0_OR_GREATER
        private static readonly Lazy<SearchValues<string>> s_lazySearchStrs = new(() => SearchValues.Create([], StringComparison.Ordinal));

        internal static SearchValues<string> GetEmptyStrings() => s_lazySearchStrs.Value;
        internal static SplitSearchValuesEntry<string> EmptyStringEntry => new(Range.All, GetEmptyStrings());
#endif
    }

    /// <summary>
    /// A read-only ref struct that represents a section of a string and the <see cref="SearchValues{T}"/> instance that it was 
    /// split by.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{{Range}\}")]
    public readonly ref struct SplitSearchValuesEntry<T> where T : IEquatable<T>
    {
        /// <summary>
        /// A range representing the start and end index of the section.
        /// </summary>
        public readonly Range Range;
        /// <summary>
        /// Gets the <see cref="SearchValues{T}"/> instance that was used to split the section.
        /// </summary>
        public readonly SearchValues<T> SplitBy { get; }

        internal SplitSearchValuesEntry(Range range, SearchValues<T> searchValues)
        {
            Range = range;
            this.SplitBy = searchValues;
        }

        /// <summary>
        /// Implicitly converts a <see cref="SplitSearchValuesEntry"/> to a <see cref="Range"/>, returning
        /// <see cref="Range"/>.
        /// </summary>
        /// <param name="entry">The entry to convert.</param>
        public static implicit operator Range(SplitSearchValuesEntry<T> entry)
        {
            return entry.Range;
        }
    }
}