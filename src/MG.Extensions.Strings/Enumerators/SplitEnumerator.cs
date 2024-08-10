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
    public ref struct SplitEnumerator
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SplitEntry _current;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlySpan<char> _splitBy;

        private ReadOnlySpan<char> _str;

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
        /// <param name="str">The string/span to split.</param>
        /// <param name="splitBy">The characters to use as a separator for splitting.</param>
        public SplitEnumerator(ReadOnlySpan<char> str, ReadOnlySpan<char> splitBy)
        {
            _current = SplitEntry.Empty;
            _str = str;
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
            ReadOnlySpan<char> span = _str;
            if (span.Length <= 0)
            {
                return false;
            }

            int index = span.IndexOf(_splitBy);
            if (index < 0)
            {
                _str = ReadOnlySpan<char>.Empty;
                _current = new SplitEntry(span, _splitBy);
                return true;
            }

            _current = new SplitEntry(span.Slice(0, index), span.Slice(index, _splitBy.Length));
            _str = span.Slice(index + _splitBy.Length);
            return true;
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
                void* pointer = Unsafe.AsPointer(ref c);
                return new ReadOnlySpan<char>(pointer, 1);
            }
        }
#endif
    }
}