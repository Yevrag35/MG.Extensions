using MG.Extensions.Guarding;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Builders
{
    /// <summary>
    /// A ref struct that provides a way to build a string from multiple segments with a combined separator.
    /// </summary>
    /// <remarks>
    /// This struct should be disposed after use to release the rented memory.
    /// </remarks>
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("Count = {Count}")]
    public ref struct SpanCharArray
    {
        const int MULTIPLE = 16;
        const int INCREMENT = MULTIPLE - 1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SpanPosition[]? _positionArray;
        private Span<SpanPosition> _positions;
        private SpanStringBuilder _builder;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _index;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isRented;
        private char _separator;

        /// <summary>
        /// Gets the read-only span at the specified index.
        /// </summary>
        /// <param name="index">The index within the array to use.</param>
        /// <returns>
        /// The <see cref="ReadOnlySpan{T}"/> at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public readonly ReadOnlySpan<char> this[int index]
        {
            get
            {
#if NET6_0_OR_GREATER
                Guard.ThrowIfNegative(index);
                Guard.ThrowIfGreaterThanOrEqual(index, _positions.Length);
#else
                Guard.ThrowIfNegative(index, nameof(index));
                Guard.ThrowIfGreaterThanOrEqual(index, _positions.Length, nameof(index));
#endif

                return this.GetSegment(index);
            }
        }
        /// <summary>
        /// Gets the capacity of this <see cref="SpanCharArray"/> instance which is the number of
        /// segments that can be added before a resize is required.
        /// </summary>
        public readonly int Capacity => _positionArray?.Length ?? _positions.Length;
        /// <summary>
        /// Gets the number of segments added to this instance.
        /// </summary>
        public readonly int Count => _index;
        /// <summary>
        /// Indicates whether this instance is using an <see cref="ArrayPool{T}"/> rented buffer.
        /// </summary>
        /// <remarks>
        /// If this <see langword="true"/>, then this array needs to be disposed 
        /// to return the buffer to the pool.
        /// </remarks>
        public readonly bool IsRented => _isRented;

        /// <summary>
        /// Initializes a new instance of <see cref="SpanCharArray"/> with the minimum initial 
        /// capacity and a separator character.
        /// </summary>
        /// <param name="minimumLength">
        /// The minimum capacity for the internal buffer the array should allocate.
        /// </param>
        /// <param name="separator">The character separator that this array will join on.</param>
        public SpanCharArray(int minimumLength, char separator)
        {
            SpanPosition[] positionArray = ArrayPool<SpanPosition>.Shared.Rent(MULTIPLE);
            _isRented = true;
            _positionArray = positionArray;
            _positions = positionArray;
            _separator = separator;
            _index = 0;
            minimumLength = Math.Max(1, minimumLength);
            _builder = new(minimumLength);
        }

        [DebuggerStepThrough]
        public SpanCharArray Add(
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> value)
        {
            this.EnsureCapacity(1);

            this.AddSegment(value);
            return this;
        }
        public SpanCharArray AddRange(
            #if NET7_0_OR_GREATER
            scoped
# endif
            ReadOnlySpan<char> value,
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> splitBy)
        {
            int count = StringExtensions.Count(value, splitBy) + 1;
            this.EnsureCapacity(count);

            foreach (ReadOnlySpan<char> section in value.SpanSplit(splitBy))
            {
                this.AddSegment(section);
            }

            return this;
        }
        private void AddSegment(
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> value)
        {
            bool noSpace = _index == 0;
            int index = _builder.Length;
            if (!noSpace)
            {
                index++;
            }

            SpanPosition pos = new(index, value.Length);
            _positions[_index++] = pos;
            if (noSpace)
            {
                _builder = _builder.Append(value);
                return;
            }

            int length = value.Length + 1;
            Span<char> buffer = stackalloc char[length];
            buffer[0] = _separator;
            value.CopyTo(buffer.Slice(1));
#if NET7_0_OR_GREATER
            _builder = _builder.Append(buffer);
#else
            unsafe
            {
#pragma warning disable CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope
                _builder = _builder.Append(buffer);
#pragma warning restore CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope
            }
#endif
        }

        public readonly ReadOnlySpan<char> AsSpan()
        {
            return _builder.AsSpan();
        }

        [DebuggerStepThrough]
        public readonly bool Contains(
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> value)
        {
            return this.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
        public readonly bool Contains(
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> value, StringComparison comparison)
        {
            int index = _index;
            for (int i = 0; i < index; i++)
            {
                if (this.GetSegment(i).Equals(value, comparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Allocates a new <see cref="string"/> from the spans added and disposes this
        /// <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <remarks>
        ///     Use <see cref="ToString"/> if you want to construct additional strings from the same array.
        /// </remarks>
        /// <returns>
        ///     The constructed <see cref="string"/> instance.
        /// </returns>
        [DebuggerStepThrough]
        public string Build()
        {
            string result = this.ToString();
            this.Dispose();
            return result;
        }
        /// <summary>
        /// Disposes this <see cref="SpanCharArray"/> and returns the internal buffer to the pool.
        /// </summary>
        public void Dispose()
        {
            bool isRented = this.IsRented;
            SpanPosition[]? array = _positionArray;
            _builder.Dispose();
            this = default;

            if (isRented)
            {
                ArrayPool<SpanPosition>.Shared.Return(array!);
            }
        }

        //TODO: Fix the Remove methods

        //        [DebuggerStepThrough]
        //        [Obsolete("A bug exists in the 'Remove' methods.", error: true)]
        //        public bool Remove(
        //#if NET7_0_OR_GREATER
        //            scoped
        //#endif
        //            ReadOnlySpan<char> value)
        //        {
        //            return this.Remove(value, StringComparison.OrdinalIgnoreCase);
        //        }
        //        [Obsolete("A bug exists in the 'Remove' methods.", error: true)]
        //        public bool Remove(
        //#if NET7_0_OR_GREATER
        //            scoped
        //#endif
        //            ReadOnlySpan<char> value, StringComparison comparison)
        //        {
        //            bool removed = false;
        //            int index = _index;
        //            for (int i = 0; i < index; i++)
        //            {
        //                ref readonly SpanPosition pos = ref _positions[i];
        //                if (_builder.GetSegment(pos.Index, pos.Length).Equals(value, comparison))
        //                {
        //                    this.RemoveAt(in i, in pos);

        //                    removed = true;
        //                    break;
        //                }
        //            }

        //            return removed;
        //        }
        //        [Obsolete("A bug exists in the 'Remove' methods.", error: true)]
        //        public void RemoveAt(int index)
        //        {
        //            ref readonly SpanPosition pos = ref _positions[index];
        //            this.RemoveAt(in index, in pos);
        //        }
        //        [Obsolete("A bug exists in the 'Remove' methods.", error: true)]
        //        private void RemoveAt(in int index, in SpanPosition pos)
        //        {
        //            if (pos.Index + pos.Length < _builder.Length)
        //            {
        //                _builder = _builder.Remove(pos.Index, pos.Length + 1); // 1 for the space
        //            }
        //            else
        //            {
        //                _builder = _builder.Remove(pos.Index, pos.Length);
        //            }

        //            Span<SpanPosition> allPos = _positions;
        //            Span<SpanPosition> posAtIndex = allPos.Slice(index);
        //            int minusOne = posAtIndex.Length - 1;

        //            for (int i = 0; i < posAtIndex.Length; i++)
        //            {
        //                posAtIndex[i] = i < minusOne
        //                    ? posAtIndex[i + 1].ShiftLeft(pos.Length)
        //                    : default;
        //            }

        //            _positions = allPos.Slice(0, allPos.Length - 1);
        //        }
        
        /// <summary>
        /// Allocates a new <see cref="string"/> from the character spans added to this <see cref="SpanCharArray"/>.
        /// </summary>
        /// <returns>
        /// The constructed <see cref="string"/> instance.
        /// </returns>
        [DebuggerStepThrough]
        public override readonly string ToString()
        {
            return _builder.ToString();
        }

        public static SpanCharArray Split(
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> value, char separator)
        {
            if (value.IsWhiteSpace())
            {
                return new(16, separator);
            }

            int count = StringExtensions.Count(value, in separator) + 1;

            SpanCharArray array = new(count, separator);

            foreach (ReadOnlySpan<char> section in value.SpanSplit(in separator))
            {
                array.AddSegment(section);
            }

            return array;
        }

        [DebuggerStepThrough]
        private static int AdjustCapacity(in int capacity)
        {
            return (capacity + INCREMENT) & ~INCREMENT;
        }
        /// <exception cref="ArgumentOutOfRangeException"/>
        [DebuggerStepThrough]
        private void EnsureCapacity(int appendLength)
        {
            int calculatedLength = _index + appendLength;
            if (calculatedLength > this.Capacity)
            {
                this.Grow(calculatedLength);
            }
        }
        [DebuggerStepThrough]
        private readonly ReadOnlySpan<char> GetSegment(int index)
        {
            ref readonly SpanPosition pos = ref _positions[index];
            return _builder.GetSegment(pos.Index, pos.Length);
        }
        [DebuggerStepThrough]
        private void Grow(int minimumCapacity)
        {
            Debug.Assert(minimumCapacity >= this.Capacity);
            int newCapacity = AdjustCapacity(in minimumCapacity);
            Debug.Assert(newCapacity % MULTIPLE == 0);

            SpanPosition[] newArray = ArrayPool<SpanPosition>.Shared.Rent(newCapacity);
            _positions.Slice(0, _index).CopyTo(newArray);
            if (this.IsRented)
            {
                ArrayPool<SpanPosition>.Shared.Return(_positionArray!);
            }
            else
            {
                _isRented = true;
            }

            _positionArray = newArray;
            _positions = newArray;
        }
    }
}
