using System.Collections;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Enumerators
{
    /// <summary>
    /// An enumerator that splits a <see cref="ReadOnlySpan{T}"/> by a <see cref="SearchValues{T}"/> of <see cref="char"/>
    /// instances and returns the sections as <see cref="SplitSearchValuesEntry{T}"/> instances.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{{_original}\}")]
    public ref struct SplitAnyEnumerator
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SplitSearchValuesEntry<char> _current;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SearchValues<char> _splitBy;

        private readonly ReadOnlySpan<char> _original;
        private readonly int _originalLength;
        private ReadOnlySpan<char> _remainingText;

        /// <summary>
        /// Gets the current <see cref="SplitSearchValuesEntry{T}"/> representing the current split section.
        /// </summary>
        public readonly SplitSearchValuesEntry<char> Current => _current;
        /// <summary>
        /// Gets the <see cref="SearchValues{T}"/> of <see cref="char"/> instances being used to split the text.
        /// </summary>
        public readonly SearchValues<char> SplitBy => _splitBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitAnyEnumerator"/> struct splitting the provided span of characters
        /// by the provided <see cref="SearchValues"/> instance.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="splitBy"></param>
        public SplitAnyEnumerator(ReadOnlySpan<char> text, SearchValues<char> splitBy)
        {
            _remainingText = text;
            _original = text;
            _originalLength = text.Length;
            _splitBy = splitBy;
            _current = SplitSearchValuesEntry.EmptyCharEntry;
        }

        /// <summary>
        /// Returns this instance of the <see cref="SplitAnyEnumerator"/> as the enumerator.
        /// </summary>
        /// <returns>
        /// The current <see cref="SplitAnyEnumerator"/> instance.
        /// </returns>
        public readonly SplitAnyEnumerator GetEnumerator() => this;

        /// <summary>
        /// Moves the enumerator to the next split section.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next section; otherwise, <see langword="false"/>.
        /// </returns>
        public bool MoveNext()
        {
            ReadOnlySpan<char> chars = _remainingText;
            bool hasNext = chars.IsEmpty;

            if (!hasNext)
            {
                hasNext = FindIndexAndSplit(chars, _originalLength, this.SplitBy, ref _remainingText, ref _current);
            }

            return hasNext;
        }

        /// <summary>
        /// Resets the enumerator to the beginning of the text.
        /// </summary>
        public void Reset()
        {
            _remainingText = _original;
            _current = SplitSearchValuesEntry.EmptyCharEntry;
        }

        private static bool FindIndexAndSplit(
            ReadOnlySpan<char> chars,
            int originalLength,
            SearchValues<char> splitBy,
            ref ReadOnlySpan<char> remainingText,
            ref SplitSearchValuesEntry<char> current)
        {
            int startAt = originalLength - chars.Length;
            int index = chars.IndexOfAny(splitBy);

            bool hasNext;
            if (index == -1)
            {
                Range range = Range.StartAt(startAt);
                remainingText = [];
                current = new(range, splitBy);
                hasNext = false;
            }
            else
            {
                Range range = new(startAt, startAt + index + 1);
                current = new(range, splitBy);

                remainingText = (uint)index + 1 < (uint)chars.Length
                    ? chars.Slice(index + 1)
                    : [];

                hasNext = true;
            }

            return hasNext;
        }
    }

#if NET9_0_OR_GREATER
    /// <summary>
    /// An enumerator that splits a <see cref="ReadOnlySpan{T}"/> by a <see cref="SearchValues{T}"/> of <see cref="string"/>
    /// instances and returns the sections as <see cref="SplitSearchValuesEntry{T}"/> instances.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{{_original}\}")]
    public ref struct SplitAnyStringEnumerator : IEnumerator<SplitSearchValuesEntry<string>>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SplitSearchValuesEntry<string> _current;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SearchValues<string>? _splitBy;

        private ReadOnlySpan<char> _original;
        private ReadOnlySpan<char> _remainingText;
        private int _originalLength;

        /// <summary>
        /// Gets the current <see cref="SplitSearchValuesEntry{T}"/> representing the current split section.
        /// </summary>
        public readonly SplitSearchValuesEntry<string> Current => _current;
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly object IEnumerator.Current => throw new NotSupportedException();
        /// <summary>
        /// Gets the <see cref="SearchValues{T}"/> of <see cref="string"/> instances being used to split the text.
        /// </summary>
        public readonly SearchValues<string> SplitBy => _splitBy ?? SplitSearchValuesEntry.GetEmptyStrings();

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitAnyStringEnumerator"/> struct splitting the provided span of characters
        /// by the provided <see cref="SearchValues"/> instance.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="splitBy"></param>
        public SplitAnyStringEnumerator(ReadOnlySpan<char> text, SearchValues<string> splitBy)
        {
            _original = text;
            _remainingText = text;
            _splitBy = splitBy;
            _current = SplitSearchValuesEntry.EmptyStringEntry;
        }

        /// <summary>
        /// Disposes of the current instance of the <see cref="SplitAnyStringEnumerator"/>.
        /// </summary>
        public void Dispose()
        {
            this = default;
        }

        /// <summary>
        /// Returns this instance of the <see cref="SplitAnyStringEnumerator"/> as the enumerator.
        /// </summary>
        /// <returns>
        /// The current <see cref="SplitAnyStringEnumerator"/> instance.
        /// </returns>
        public readonly SplitAnyStringEnumerator GetEnumerator() => this;

        /// <summary>
        /// Moves the enumerator to the next split section.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next section; otherwise, <see langword="false"/>.
        /// </returns>
        public bool MoveNext()
        {
            ReadOnlySpan<char> chars = _remainingText;
            bool hasNext = chars.IsEmpty;

            if (!hasNext)
            {
                hasNext = FindIndexAndSplit(chars, _originalLength, this.SplitBy, ref _remainingText, ref _current);
            }

            return hasNext;
        }

        /// <summary>
        /// Resets the enumerator to the beginning of the text.
        /// </summary>
        public void Reset()
        {
            _remainingText = _original;
            _current = SplitSearchValuesEntry.EmptyStringEntry;
        }

        private static bool FindIndexAndSplit(
            ReadOnlySpan<char> chars,
            int originalLength,
            SearchValues<string> splitBy,
            ref ReadOnlySpan<char> remainingText,
            ref SplitSearchValuesEntry<string> current)
        {
            int startAt = originalLength - chars.Length;
            int index = chars.IndexOfAny(splitBy);

            bool hasNext;
            if (index == -1)
            {
                Range range = Range.StartAt(startAt);
                remainingText = [];
                current = new(range, splitBy);
                hasNext = false;
            }
            else
            {
                Range range = new(startAt, startAt + index + 1);
                current = new(range, splitBy);

                remainingText = (uint)index + 1u < (uint)chars.Length
                    ? chars.Slice(index + 1)
                    : [];

                hasNext = true;
            }

            return hasNext;
        }
    }
#endif
}