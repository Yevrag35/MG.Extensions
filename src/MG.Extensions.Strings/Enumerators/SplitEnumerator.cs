using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Enumerators
{
    /// <summary>
    /// Provides an enumerator for splitting a <see cref="ReadOnlySpan{T}"/> of <see cref="char"/> elements
    /// based on specified separator characters.
    /// </summary>
    [DebuggerStepThrough]
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{Current = {Current}\}")]
    public ref struct SplitEnumerator
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SplitEntry _current;
        private readonly int _originalLength;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlySpan<char> _splitBy;

        private ReadOnlySpan<char> _remainingText;

        /// <summary>
        /// Gets the current <see cref="SplitEntry"/> in the enumeration.
        /// </summary>
        public readonly SplitEntry Current => _current;
        /// <summary>
        /// Gets the separator characters being used for splitting.
        /// </summary>
        public readonly ReadOnlySpan<char> SplitChars => _splitBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitEnumerator"/> struct.
        /// </summary>
        /// <param name="text">The string/span to split.</param>
        /// <param name="splitBy">The character to use as a separator for splitting.</param>
        public SplitEnumerator(ReadOnlySpan<char> text, in char splitBy)
            : this(text, splitBy: FromOneChar(in splitBy))
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitEnumerator"/> struct.
        /// </summary>
        /// <param name="text">The string/span to split.</param>
        /// <param name="splitBy">The characters to use as a separator for splitting.</param>
        public SplitEnumerator(ReadOnlySpan<char> text, ReadOnlySpan<char> splitBy)
        {
            _current = SplitEntry.Empty;
            _originalLength = text.Length;
            _remainingText = text;
            _splitBy = splitBy;
        }

        /// <summary>
        /// Returns the enumerator itself, as required by the compiler for the foreach syntax.
        /// </summary>
        /// <returns>The <see cref="SplitEnumerator"/> instance.</returns>
        public readonly SplitEnumerator GetEnumerator() => this;

        /// <summary>
        /// Advances the enumerator to the next split entry.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the enumerator was successfully advanced to the next entry; 
        ///     <see langword="false"/> if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            ReadOnlySpan<char> chars = _remainingText;
            bool hasNext = chars.IsEmpty;

            if (!hasNext)
            {
                hasNext = FindIndexAndSplit(chars, _splitBy, in _originalLength, ref _remainingText, ref _current);
            }

            return hasNext;
        }

        private static bool FindIndexAndSplit(ReadOnlySpan<char> chars, ReadOnlySpan<char> splitBy, in int originalLength, ref ReadOnlySpan<char> remainingText, ref SplitEntry current)
        {
            int startAt = originalLength - chars.Length;
            int index = chars.IndexOfAny(splitBy);

            bool hasNext;
            if (index == -1)
            {
                Range range = Range.StartAt(startAt);
                remainingText = ReadOnlySpan<char>.Empty;
                current = new(range, chars, splitBy);
                hasNext = false;
            }
            else
            {
                Range range = new(startAt, startAt + index + 1);
                current = new(range, chars.Slice(0, index), splitBy);

                remainingText = (uint)index + 1u < (uint)chars.Length
                    ? chars.Slice(index + 1)
                    : ReadOnlySpan<char>.Empty;

                hasNext = true;
            }

            return hasNext;
        }

        internal static ReadOnlySpan<char> FromOneChar(in char value)
        {
#if NETSTANDARD2_1 || NET6_0
            ref char c = ref Unsafe.AsRef(in value);
            return MemoryMarshal.CreateReadOnlySpan(ref c, 1);
        }
#elif NET7_0_OR_GREATER
            return new ReadOnlySpan<char>(in value);
        }
#else
            return ReinterpretReadonly(in value);
        }

        private static ReadOnlySpan<char> ReinterpretReadonly(in char value)
        {
            ref char c = ref Unsafe.AsRef(in value);
            unsafe
            {
                fixed (char* pointer = &c)
                {
                    return new ReadOnlySpan<char>(pointer, 1);
                }
            }
        }
#endif
    }
}