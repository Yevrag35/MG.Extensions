using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Enumerators
{
    /// <summary>
    /// Represents an entry in a split sequence, containing both the characters before the separator and 
    /// the separator itself.
    /// </summary>
    [DebuggerStepThrough]
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{{Section}\}")]
    public readonly ref struct SplitEntry
    {
        /// <summary>
        /// Gets the slice of characters before the separator.
        /// </summary>
        public readonly ReadOnlySpan<char> Section;
        /// <summary>
        /// The range containing the inclusive start and exclusive end indexes of the entry.
        /// </summary>
        public readonly Range Range;
        /// <summary>
        /// Gets the separator characters used.
        /// </summary>
        public readonly ReadOnlySpan<char> Separator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitEntry"/> struct.
        /// </summary>
        /// <param name="range">
        /// The range containing the inclusive start and exclusive end indexes of the entry.
        /// </param>
        /// <param name="chars">The characters before the separator.</param>
        /// <param name="separator">The separator characters.</param>
        internal SplitEntry(Range range, ReadOnlySpan<char> chars, ReadOnlySpan<char> separator)
        {
            Range = range;
            Section = chars;
            Separator = separator;
        }

        /// <summary>
        /// Deconstructs the <see cref="SplitEntry"/> into its component characters and separator.
        /// </summary>
        /// <param name="range">
        /// When this method returns, contains the range containing the inclusive start and exclusive end indexes of the entry.
        /// </param>
        /// <param name="chars">When this method returns, contains the characters before the separator.</param>
        /// <param name="separator">When this method returns, contains the separator characters.</param>
        public readonly void Deconstruct(out Range range, out ReadOnlySpan<char> chars, out ReadOnlySpan<char> separator)
        {
            range = Range;
            chars = Section;
            separator = Separator;
        }

        /// <summary>
        /// An empty <see cref="SplitEntry"/> instance with empty character and separator spans.
        /// </summary>
        public static SplitEntry Empty => new SplitEntry(Range.All, ReadOnlySpan<char>.Empty, ReadOnlySpan<char>.Empty);

        /// <summary>
        /// Implicitly converts a <see cref="SplitEntry"/> to a <see cref="ReadOnlySpan{T}"/>, returning 
        /// its characters before the separator.
        /// </summary>
        /// <param name="entry">The split entry to convert.</param>
        public static implicit operator ReadOnlySpan<char>(SplitEntry entry) => entry.Section;
        /// <summary>
        /// Implicitly converts a <see cref="SplitEntry"/> to a <see cref="Range"/>, returning
        /// a range representing the inclusive start and exclusive end indexes of the entry.
        /// </summary>
        /// <param name="entry"></param>
        public static implicit operator Range(SplitEntry entry) => entry.Range;
    }
}