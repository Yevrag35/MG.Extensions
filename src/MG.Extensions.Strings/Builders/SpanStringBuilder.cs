using MG.Extensions.Guarding;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Builders
{
    /// <summary>
    /// A ref struct that provides an efficient way to build strings using expandable 
    /// <see cref="Span{T}"/> buffers.
    /// </summary>
    /// <remarks>
    /// This struct should be disposed after use to release the rented memory.
    /// </remarks>
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{AsSpan()\}")]
    public ref partial struct SpanStringBuilder
    {
        const int MULTIPLE = 128;
        const int INCREMENT = MULTIPLE - 1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static readonly string NEW_LINE = Environment.NewLine;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static readonly int NEW_LINE_LENGTH = NEW_LINE.Length;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isRented;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _position;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private char[]? _array;
        private Span<char> _span;

        /// <summary>
        /// Gets the reference to the character at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>
        /// The <see cref="char"/> element at the specified index.
        /// </returns>
        /// <inheritdoc cref="Span{T}.this[int]" path="/exception"/>
        public ref char this[int index] => ref _span[index];

        /// <summary>
        /// Gets the capacity of the current internal buffer.
        /// </summary>
        public readonly int Capacity => _array?.Length ?? _span.Length;
        /// <summary>
        /// Indicates whether this builder is using an <see cref="ArrayPool{T}"/> rented buffer.
        /// </summary>
        /// <remarks>
        /// If this <see langword="true"/>, then this builder needs to be disposed 
        /// to return the buffer to the pool.
        /// </remarks>
        public readonly bool IsRented => _isRented;
        /// <summary>
        /// Gets the number of characters written to the current builder.
        /// </summary>
        public readonly int Length => _position;

        /// <summary>
        /// Initializes a new instance of <see cref="SpanStringBuilder"/> with the specified
        /// minimum initial capacity.
        /// </summary>
        /// <param name="minimumCapacity">
        ///     The minimum capacity for the internal buffer the builder should allocate.
        /// </param>
        public SpanStringBuilder(int minimumCapacity)
        {
            AdjustCapacity(ref minimumCapacity);
            char[] array = ArrayPool<char>.Shared.Rent(minimumCapacity);
            _array = array;
            _isRented = true;
            _span = array;
            _position = 0;
        }
        /// <summary>
        /// Initializes a new instance of <see cref="SpanStringBuilder"/> with the specified
        /// buffer to use internally.
        /// </summary>
        /// <param name="initialBuffer">
        /// The initial buffer to use for the builder. By providing a buffer, the builder will not
        /// return it to the <see cref="ArrayPool{T}"/> when disposed.
        /// <para>
        /// It will rent a new buffer, however, if this builder exceeds its capacity.
        /// </para>
        /// </param>
        [DebuggerStepThrough]
        public SpanStringBuilder(Span<char> initialBuffer)
        {
            _array = null;
            _isRented = false;
            _span = initialBuffer;
            _position = 0;
        }

        /// <summary>
        /// Allocates a new <see cref="string"/> from the characters appended to the builder and disposes this
        /// <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <remarks>
        ///     Use <see cref="ToString"/> if you want to construct additional strings from the same builder.
        /// </remarks>
        /// <returns>
        ///     The constructed <see cref="string"/> instance.
        /// </returns>
        public string Build()
        {
            string str = this.ToString();
            this.Dispose();
            return str;
        }

        /// <inheritdoc cref="StringBuilder.Append(char)" path="/*[not(self::returns)]"/>
        /// <returns>
        /// This same instance of <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        public SpanStringBuilder Append(char value)
        {
            this.EnsureCapacity(1);
            _span[_position++] = value;
            return this;
        }

        /// <inheritdoc cref="StringBuilder.Append(char, int)" path="/*[not(self::returns)]"/>
        /// <returns>
        /// This same instance of <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        public SpanStringBuilder Append(char value, int count)
        {
            this.EnsureCapacity(count);

            int pos = _position;
            _span.Slice(pos, count).Fill(value);
            _position += count;

            return this;
        }

        /// <summary>
        /// Appends the specified read-only character span to this instance.
        /// </summary>
        /// <returns>
        /// The same instance of this <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        /// <inheritdoc cref="EnsureCapacity(int)" path="/exception"/>
        public SpanStringBuilder Append(
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return this;
            }

            int pos = _position;
            this.EnsureCapacity(value.Length);
            value.CopyToSlice(_span, ref pos);

            _position = pos;
            return this;
        }

        /// <summary>
        /// Appends the default line terminator to the end of the current
        /// <see cref="SpanStringBuilder"/> instance.
        /// </summary>
        /// <returns>
        /// This same instance of <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        /// <inheritdoc cref="EnsureCapacity(int)" path="/exception"/>
        public SpanStringBuilder AppendLine()
        {
            this.EnsureCapacity(NEW_LINE_LENGTH);
            NEW_LINE.CopyToSlice(_span, ref _position);
            return this;
        }
        /// <summary>
        /// Appends a copy of the specified <see cref="string"/> followed by the default line
        /// terminator to the end of the current <see cref="SpanStringBuilder"/> instance.
        /// </summary>
        /// <returns>
        /// This same instance of <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        /// <inheritdoc cref="EnsureCapacity(int)" path="/exception"/>
        public SpanStringBuilder AppendLine(
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> value)
        {
            this.EnsureCapacity(value.Length + NEW_LINE_LENGTH);
            value.CopyToSlice(_span, ref _position);
            NEW_LINE.CopyToSlice(_span, ref _position);

            return this;
        }
        /// <summary>
        /// Forms a slice out of the buffer for this builder representing the currently
        /// written characters.
        /// </summary>
        /// <returns>
        /// A span consisting of <see cref="Length"/> element(s).
        /// </returns>
        public readonly Span<char> AsSpan()
        {
            return _span.Slice(0, _position);
        }

        /// <summary>
        /// Copies the contents of this builder to a destination span.
        /// </summary>
        /// <param name="destination">The destination span to copy the contents to.</param>
        /// <returns>
        /// The number of <see cref="char"/> elements copied to the destination span.
        /// </returns>
        public readonly int CopyTo(
#if NET7_0_OR_GREATER
            scoped
#endif
            Span<char> destination)
        {
            _span.Slice(0, _position).CopyTo(destination);
            return _position;
        }

        //public readonly ReadOnlySpan<char> GetSegment(int start, int length)
        //{
        //    return _span.Slice(start, length);
        //}

        /// <summary>
        /// Searches for the specified character and returns the index of its first occurrence.
        /// </summary>
        /// <param name="value">The character to search for.</param>
        /// <returns>
        /// The index of the occurrence of the specified character, or, if not found, -1.
        /// </returns>
        [DebuggerStepThrough]
        public readonly int IndexOf(char value)
        {
            return _span.Slice(0, _position).IndexOf(value);
        }

        /// <summary>
        /// Inserts the specified character at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the character.</param>
        /// <param name="c">The character to insert.</param>
        /// <returns>
        /// This same instance of <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        /// <inheritdoc cref="EnsureCapacity(int)" path="/exception"/>
        [DebuggerStepThrough]
        public SpanStringBuilder Insert(int index, char c)
        {
            return this.Insert(index, c, 1);
        }
        /// <summary>
        /// Inserts the specified character <paramref name="count"/> number times at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the character(s).</param>
        /// <param name="c">The character to insert.</param>
        /// <param name="count">The number of times <paramref name="c"/> is inserted.</param>
        /// <returns>
        /// This same instance of <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        /// <inheritdoc cref="EnsureCapacity(int)" path="/exception"/>
        public SpanStringBuilder Insert(int index, char c, int count)
        {
            this.EnsureCapacity(count);
            int remaining = _position - index;

            _span.Slice(index, remaining).CopyTo(_span.Slice(index + count));
            _span.Slice(index, count).Fill(c);

            _position += count;
            return this;
        }
        /// <summary>
        /// Inserts the specified read-only span of characters at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the value.</param>
        /// <param name="value">The span of characters to insert.</param>
        /// <returns>
        /// This same instance of <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        /// <inheritdoc cref="EnsureCapacity(int)" path="/exception"/>
        public SpanStringBuilder Insert(int index,
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return this;
            }

            this.EnsureCapacity(value.Length);
            int remaining = _position - index;
            _span.Slice(index, remaining).CopyTo(_span.Slice(index + value.Length));
            value.CopyTo(_span.Slice(index));

            _position += value.Length;
            return this;
        }

        /// <summary>
        /// Removes all of the characters starting from the specified index for <paramref name="length"/>
        /// number of characters, shifting the remaining characters to the left.
        /// </summary>
        /// <param name="startIndex">The zero-based index the removal starts from.</param>
        /// <param name="length">The number of characters that will be removed.</param>
        /// <returns>
        /// This same instance of <see cref="SpanStringBuilder"/> for chaining.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="startIndex"/> and/or <paramref name="length"/> are out of range.
        /// </exception>
        public SpanStringBuilder Remove(int startIndex, int length)
        {
            int position = _position;
            int newLength = startIndex + length;
            // Validate parameters
#if NET6_0_OR_GREATER
            Guard.ThrowIfNegative(startIndex);
            Guard.ThrowIfNegative(length);
            Guard.ThrowIfGreaterThanOrEqual(startIndex, position);
#else
            Guard.ThrowIfNegative(startIndex, nameof(startIndex));
            Guard.ThrowIfNegative(length, nameof(length));
            Guard.ThrowIfGreaterThanOrEqual(startIndex, position, nameof(startIndex));
#endif
            Guard.ThrowIfGreaterThan(newLength, position, paramName: "'startIndex' and 'length'");

            // Calculate the number of elements to move
            int moveCount = position - newLength;

            // Shift elements to the left; CopyTo handles overlapping memory regions correctly
            if (moveCount > 0)
            {
                _span.Slice(newLength, moveCount).CopyTo(_span.Slice(startIndex));
            }

            // Update position
            _position -= length;
            return this;
        }

        /// <summary>
        /// Allocates a new <see cref="string"/> from the characters appended to the builder.
        /// </summary>
        /// <returns>
        /// The constructed <see cref="string"/> instance.
        /// </returns>
        [DebuggerStepThrough]
        public override readonly string ToString()
        {
            string s = this.AsSpan().ToString();
            return s;
        }

        private static void AdjustCapacity(ref int capacity)
        {
            capacity = (capacity + INCREMENT) & ~INCREMENT;
        }
        /// <exception cref="ArgumentOutOfRangeException"/>
        private void EnsureCapacity(int appendLength)
        {
            int calculatedLength = _position + appendLength;
            if (calculatedLength > this.Capacity)
            {
                this.Grow(calculatedLength);
            }
        }
        internal readonly ReadOnlySpan<char> GetSegment(int start, int length)
        {
            return _span.Slice(start, length);
        }
        private void Grow(int newCapacity)
        {
            Debug.Assert(newCapacity >= this.Capacity);
            AdjustCapacity(ref newCapacity);
            Debug.Assert(newCapacity % MULTIPLE == 0);

            char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
            _span.Slice(0, _position).CopyTo(newArray);
            if (this.IsRented)
            {
                ArrayPool<char>.Shared.Return(_array!);
            }
            else
            {
                _isRented = true;
            }

            _array = newArray;
            _span = newArray;
        }

        /// <summary>
        /// Disposes of this <see cref="SpanStringBuilder"/> instance, returning the 
        /// buffer to the <see cref="ArrayPool{T}"/> if it was rented.
        /// </summary>
        public void Dispose()
        {
            char[]? array = _array;
            bool isRented = _isRented;
            this = default;

            if (isRented)
            {
                ArrayPool<char>.Shared.Return(array!);
            }
        }
    }
}
