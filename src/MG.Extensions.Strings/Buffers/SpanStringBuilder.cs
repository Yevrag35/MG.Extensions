using MG.Extensions.Guarding;
using MG.Extensions.Strings.Unmanaged;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace MG.Extensions.Strings.Buffers
{
    /// <summary>
    /// A ref struct that provides a way to build strings without allocating memory on the heap until the <see cref="string"/> is constructed.
    /// </summary>
    /// <remarks>
    /// The <see cref="SpanStringBuilder"/> utilizes stack-allocated buffers and pooled arrays to avoid or reuse heap allocations during the building process.
    /// </remarks>
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("{ToString(),nq}")]
    public ref struct SpanStringBuilder
    {
        internal const int DEFAULT_CAPACITY = 128;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static readonly string s_newLine = Environment.NewLine;
        static readonly int s_newLineLength = s_newLine.Length;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _position;

        private RentedBuffer<char> _buffer;

        /// <summary>
        /// Gets a reference to the character at the specified index within the current written buffer.
        /// </summary>
        /// <param name="index">The zero-based index of the character.</param>
        /// <value>The reference to the character at the specified index.</value>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public readonly ref char this[int index]
        {
            get
            {
                Guard.ThrowIfNegativeOrGreaterThan(index, (uint)_position, nameof(index));
                //return ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer.Buffer), index);
            }
        }

        /// <summary>
        /// Gets the total capacity of the current buffer.
        /// </summary>
        /// <value>
        /// The total number of characters that the <see cref="SpanStringBuilder"/> can currently store before resizing.
        /// </value>
        public readonly int Capacity => _buffer.Length;
        /// <summary>
        /// Gets or sets a value indicating whether the buffer should be cleared when the <see cref="SpanStringBuilder"/> is disposed.
        /// </summary>
        public bool ClearOnDispose
        {
            readonly get => _buffer.ClearOnDispose;
            set => _buffer.ClearOnDispose = value;
        }
        /// <summary>
        /// Indicates whether this <see cref="SpanStringBuilder"/> instance is default-initialized.
        /// </summary>
        public readonly bool IsDefaultOrEmpty => _buffer.IsDefaultOrEmpty;
        /// <summary>
        /// Gets a value indicating whether the internal buffer is rented from the <see cref="ArrayPool{T}"/>.
        /// </summary>
        /// <value><see langword="true"/> if the buffer is rented; otherwise, <see langword="false"/>.</value>
        public readonly bool IsRented => _buffer.IsRented;
        /// <summary>
        /// Gets the number of characters appended to the <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <value>The number of characters currently in the buffer.</value>
        public readonly int Length => _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanStringBuilder"/> struct with a specified initial capacity.
        /// </summary>
        /// <param name="minimumCapacity">The initial capacity for the buffer.</param>
        public SpanStringBuilder(int minimumCapacity)
        {
            int capacity = Math.Max(DEFAULT_CAPACITY / 2, minimumCapacity);
            capacity = BitHelper.RoundUpToPowerOf2Unsafe(capacity);

            RentedBuffer<char> buffer = new(capacity, useEntireCapacity: true);
            _buffer = buffer;
            _position = 0;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanStringBuilder"/> struct with a pre-allocated buffer.
        /// </summary>
        /// <param name="initialBuffer">A span that provides the initial storage for the builder.</param>
        [DebuggerStepThrough]
        public SpanStringBuilder(Span<char> initialBuffer)
        {
            var buffer = RentedBuffer.RentSpan(initialBuffer);
            _buffer = buffer;
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

        /// <summary>
        /// Appends a character to the <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <param name="value">The character to append.</param>
        public void Append(char value)
        {
            this.EnsureCapacity(1);
            _buffer[_position++] = value;
        }
        /// <summary>
        /// Appends a specified number of copies of the given character to the <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <param name="value">The character to append.</param>
        /// <param name="count">The number of times to append the character.</param>
        public void Append(char value, int count)
        {
            Debug.Assert(count > 0, "Count is 0 or negative!");
            count = Math.Max(0, count);
            this.EnsureCapacity(count);

            ref int pos = ref _position;
            _buffer.Slice(pos, count).Fill(value);
            pos += count;
        }
#if NET8_0_OR_GREATER
        /// <summary>
        /// Appends the string representation of the specified enumeration value to the current instance.
        /// </summary>
        /// <remarks>This method formats the enumeration value as a string and appends it to the builder.  If the
        /// enumeration value cannot be formatted within the initial buffer size, the buffer is resized dynamically.</remarks>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an unmanaged enumeration type.</typeparam>
        /// <param name="enumValue">The enumeration value to append.</param>
        public void Append<TEnum>(TEnum enumValue) where TEnum : unmanaged, Enum
        {
            RentedBuffer<char> buffer = new(stackalloc char[DEFAULT_CAPACITY / 2]);
            try
            {
                int written = 0;
                while (!Enum.TryFormat(enumValue, buffer, out written))
                {
                    int length = buffer.Length;
                    buffer.Resize(length * 2);
                }

                this.Append(buffer.Slice(0, written));
            }
            finally
            {
                buffer.Dispose();
            }
        }
#endif
        /// <summary>
        /// Appends the string representation of the specified <see cref="Guid"/> to the current buffer.
        /// </summary>
        /// <remarks>The method ensures that the buffer has sufficient capacity to accommodate the appended
        /// value.</remarks>
        /// <param name="value">The <see cref="Guid"/> to append.</param>
        /// <param name="format">An optional format specifier that defines the format of the appended <see cref="Guid"/>.  If not specified, the
        /// default format is used.</param>
        /// <inheritdoc cref="LengthConstants.GetGuidLength(ReadOnlySpan{char})" path="/exception"/>
        /// <exception cref="OutOfMemoryException"/>
        public void Append(Guid value, ReadOnlySpan<char> format = default)
        {
            int length = LengthConstants.GetGuidLength(format);
            this.EnsureCapacity(length);

            var slice = _buffer.Slice(_position);

#if NETSTANDARD2_0
            GuidFormatting.TryFormat(value, slice, out int written, format);
#else
            _ = value.TryFormat(slice, out int written, format: format);
#endif
            _position += written;

            Debug.Assert(!slice.Slice(0, written).Contains(default), "This is where you're messing up!");
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Appends the specified binary integer value to the current buffer, using the provided format provider if
        /// specified.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the binary integer to append.
        /// </typeparam>
        /// <param name="number">The binary integer value to append to the buffer.</param>
        /// <param name="provider">An optional format provider that supplies culture-specific formatting information. If null, the default
        /// formatting is used.</param>
        public void Append<T>(T number, IFormatProvider? provider = null) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
        {
            int length = number.GetLength();
            this.EnsureCapacity(length);
            _position = number.CopyToSlice(_buffer, _position, default, provider);
        }
#endif
        /// <summary>
        /// Appends the specified 32-bit signed integer value to the current buffer.
        /// </summary>
        /// <param name="value">The 32-bit signed integer value to append to the buffer.</param>
        public void Append(int value)
        {
            int length = value.GetLength();
            this.EnsureCapacity(length);

            _position = value.CopyToSlice(_buffer, _position);
        }

        /// <summary>
        /// Appends the characters from the specified <see cref="ReadOnlySpan{T}"/> to the current instance.
        /// </summary>
        /// <param name="value">The span containing the characters to append.</param>
        public void Append(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return;
            }

            this.EnsureCapacity(value.Length);
            _position = value.CopyToSlice(_buffer, _position);
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Appends the string representation of a specified <see cref="ISpanFormattable"/> value to this instance.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="ISpanFormattable"/> being appended.</typeparam>
        /// <param name="formattable">The formattable value to append.</param>
        /// <param name="maxLength">The maximum length of the formatted string.</param>
        /// <param name="format">An optional format string.</param>
        /// <param name="provider">An optional format provider.</param>
        public void Append<T>(T formattable, int maxLength, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : ISpanFormattable
        {
            this.EnsureCapacity(maxLength);
            _position = formattable.CopyToSlice(_buffer, _position, format, provider);
        }
#endif
#if NET9_0_OR_GREATER
        /// <summary>
        /// Appends the result of the provided <see cref="Action{T, TState}"/> to the current instance.
        /// </summary>
        /// <typeparam name="T">The type of state object passed to the action.</typeparam>
        /// <param name="length">The number of characters to be written.</param>
        /// <param name="state">The state passed to the <paramref name="spanAction"/>.</param>
        /// <param name="spanAction">The action that writes to the span.</param>
        public void Append<T>(int length, T state, Action<Span<char>, T> spanAction) where T : allows ref struct
        {
            this.EnsureCapacity(length);
            ref int pos = ref _position;
            spanAction(_buffer.Slice(pos, length), state);
            pos += length;
        }
#endif
        /// <summary>
        /// Appends the result of the provided <see cref="Action{T, TState}"/> to the current instance.
        /// </summary>
        /// <typeparam name="T">The type of state object passed to the action.</typeparam>
        /// <param name="length">The number of characters to be written.</param>
        /// <param name="state">The state passed to the <paramref name="spanAction"/>.</param>
        /// <param name="spanAction">The action that writes to the span.</param>
        internal unsafe void Append<T>(int length, T state, delegate*<Span<char>, T, void> spanAction)
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif
        {
            this.EnsureCapacity(length);
            ref int pos = ref _position;
            spanAction(_buffer.Slice(pos, length), state);
            pos += length;
        }
#if NET9_0_OR_GREATER
        /// <summary>
        /// Appends the result of the provided delegate to the current instance.
        /// </summary>
        /// <typeparam name="T">The type of state object passed to the function.</typeparam>
        /// <param name="maxLength">The maximum number of characters that may be written.</param>
        /// <param name="state">The state passed to the <paramref name="spanFunc"/>.</param>
        /// <param name="spanFunc">The delegate that writes to the span.</param>
        public void Append<T>(int maxLength, T state, Func<Span<char>, T, int> spanFunc)where T : allows ref struct
        {
            this.EnsureCapacity(maxLength);
            int written = spanFunc(_buffer.Slice(_position, maxLength), state);
            _position += written;
        }
#endif
        /// <summary>
        /// Appends a formatted value to the current buffer using a user-provided callback function.
        /// </summary>
        /// <remarks>This method ensures that the buffer has sufficient capacity to accommodate <paramref
        /// name="maxLength"/> characters before invoking the callback function. The buffer's position is updated based on the
        /// number of characters written by the callback.</remarks>
        /// <typeparam name="T">The type of the state object passed to the callback function. Must be a ref struct.</typeparam>
        /// <param name="maxLength">The maximum number of characters that can be written to the buffer. Must be a positive value.</param>
        /// <param name="state">A state object that is passed to the callback function to provide additional context or data.</param>
        /// <param name="funcPtr">A pointer to a callback function that writes the formatted value to the provided <see cref="Span{T}"/>. The
        /// function must return the number of characters written.</param>
        internal unsafe void Append<T>(int maxLength, T state, delegate*<Span<char>, T, int> funcPtr)
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif
        {
            this.EnsureCapacity(maxLength);
            var slice = _buffer.Slice(_position, maxLength);
            int written = funcPtr(slice, state);
            _position += written;

            Debug.Assert(!slice.Slice(0, written).Contains(default), "This is where you're messing up.");
        }
        internal unsafe void Append<T>(int maxLength, ref T state, delegate*<Span<char>, ref T, int> funcPtr)
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif
        {
            this.EnsureCapacity(maxLength);
            var slice = _buffer.Slice(_position, maxLength);
            int written = funcPtr(slice, ref state);
            _position += written;

            Debug.Assert(!slice.Slice(0, written).Contains(default), "This is where you're messing up.");
        }

        /// <summary>
        /// Appends UTF-8 encoded text to the <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <param name="utf8Text">The UTF-8 encoded bytes to append.</param>
        /// <returns>The current instance after the text has been appended.</returns>
        public void Append(ReadOnlySpan<byte> utf8Text)
        {
            if (utf8Text.IsEmpty)
            {
                return;
            }

            int maxLength = Encoding.UTF8.GetMaxCharCount(utf8Text.Length);
            this.EnsureCapacity(maxLength);

            _position = utf8Text.CopyToSlice(_buffer, _position);
        }
        /// <summary>
        /// Appends the specified spans of characters to the current instance.
        /// </summary>
        /// <remarks>This method allows appending multiple spans of characters in a single call.  The operation is
        /// performed efficiently using inlining to minimize overhead.</remarks>
        /// <param name="values">An array of <see cref="ReadOnlySpan{T}"/> of characters to append. Each span in the array is appended in order.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendChars(ReadOnlySpan<char> values)
        {
            this.Append(values);
        }
        /// <summary>
        /// Appends a new line to the <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <returns>The current instance after the new line has been appended.</returns>
        public void AppendLine()
        {
            this.EnsureCapacity(s_newLineLength);
            _position = s_newLine.CopyToSlice(_buffer, _position);
        }
        /// <summary>
        /// Appends a span of characters followed by a new line to the <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <param name="value">The span of characters to append.</param>
        public void AppendLine(ReadOnlySpan<char> value)
        {
            this.EnsureCapacity(value.Length + s_newLineLength);

            int pos = value.CopyToSlice(_buffer, _position);
            _position = s_newLine.CopyToSlice(_buffer, pos);
        }
        /// <summary>
        /// Appends a span of characters of the specified length to the current buffer.
        /// </summary>
        /// <param name="length">The number of characters to append. Must be non-negative and within the available capacity.</param>
        /// <returns>A <see cref="Span{T}"/> of characters representing the appended section of the buffer that can be written to.</returns>
        public Span<char> AppendSpan(int length)
        {
            this.EnsureCapacity(length);
            Span<char> slice = _buffer.Slice(_position, length);
            slice.Clear();

            _position += length;
            return slice;
        }

        /// <summary>
        /// Returns a span representing the current contents of the <see cref="SpanStringBuilder"/>.
        /// </summary>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> representing the characters in the builder.</returns>
        public readonly ReadOnlySpan<char> AsSpan()
        {
            return _buffer.Slice(0, _position);
        }
        /// <summary>
        /// Returns a read-only span of characters from the underlying buffer, starting at the specified position and spanning
        /// the specified length.
        /// </summary>
        /// <param name="start">The zero-based starting position of the span within the buffer.</param>
        /// <param name="length">The number of characters to include in the span.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> of characters representing the specified range of the buffer.</returns>
        /// <inhertdoc cref="Span{T}.Slice(int, int)" path="/exception"/>
        public readonly ReadOnlySpan<char> AsSpan(int start, int length)
        {
            return _buffer.Slice(start, length);
        }

        /// <summary>
        /// Clears all data from the builder and resets the length to <c>0</c>.
        /// </summary>
        /// <remarks>This method removes all elements from the underlying buffer and sets the position to zero. After
        /// calling this method, the buffer will be empty, and any subsequent operations will start from the beginning of the
        /// buffer.</remarks>
        public void Clear()
        {
            _position = 0;
        }
        //}
        /// <summary>
        /// Copies the contents of this builder to a destination span.
        /// </summary>
        /// <param name="destination">The destination span to copy the contents to.</param>
        /// <returns>
        /// The number of <see cref="char"/> elements copied to the destination span.
        /// </returns>
        public readonly int CopyTo(Span<char> destination)
        {
            _buffer.Slice(0, _position).CopyTo(destination);
            return _position;
        }

        /// <summary>
        /// Releases the resources used by the <see cref="SpanStringBuilder"/>, returning any rented buffers to the pool.
        /// </summary>
        /// <remarks>
        /// This is automatically called when invoking <see cref="Build"/>.
        /// </remarks>
        public void Dispose()
        {
            _buffer.Dispose();
            this = default;
        }
        public readonly bool EndsWith(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return _position > 0 && _position >= value.Length
                && _buffer.Slice(0, _position).EndsWith(value, comparisonType);
        }
        /// <summary>
        /// Returns a slice of the current buffer starting at the specified index and length.
        /// </summary>
        /// <param name="start">The starting index of the slice.</param>
        /// <param name="length">The length of the slice.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> representing the specified slice.</returns>
        public readonly Span<char> GetSegment(int start, int length)
        {
            return _buffer.Slice(start, length);
        }
        /// <summary>
        /// Returns a read-only span of characters representing the segment defined by the specified position.
        /// </summary>
        /// <param name="position">The position that defines the start and end of the segment to retrieve. Must specify a valid range within
        /// the underlying buffer.</param>
        /// <returns>A read-only span of characters corresponding to the segment indicated by <paramref name="position"/>.</returns>
        public readonly ReadOnlySpan<char> GetSegment(SpanPosition position)
        {
            return _buffer[position.ToRange()];
        }

        /// <summary>
        /// Finds the index of the specified character within the current buffer.
        /// </summary>
        /// <param name="value">The character to locate.</param>
        /// <returns>The zero-based index of the first occurrence of the character, or -1 if not found.</returns>
        [DebuggerStepThrough]
        public readonly int IndexOf(char value)
        {
            return _buffer.Buffer.IndexOf(value);
        }
        /// <summary>
        /// Finds the index of the specified character within the current buffer, starting from the specified index.
        /// </summary>
        /// <param name="value">The character to locate.</param>
        /// <param name="startIndex">The zero-based index at which to begin the search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of the character, or -1 if the character is not found.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="startIndex"/> is less than 0 or greater than or equal to the length of the span.
        /// </exception>
        [DebuggerStepThrough]
        public readonly int IndexOf(char value, int startIndex)
        {
            return _buffer.Slice(startIndex).IndexOf(value);
        }
        /// <summary>
        /// Finds the index of the specified character within the current buffer, starting from the specified index and searching up to the specified count of characters.
        /// </summary>
        /// <param name="value">The character to locate.</param>
        /// <param name="startIndex">The zero-based index at which to begin the search.</param>
        /// <param name="count">The maximum number of characters to examine during the search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of the character, or -1 if the character is not found within the specified range.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="startIndex"/> is less than 0 or greater than or equal to the length of the span, or if <paramref name="count"/> is less 
        /// than 0 or exceeds the number of characters from <paramref name="startIndex"/> to the end of the buffer.
        /// </exception>
        [DebuggerStepThrough]
        public readonly int IndexOf(char value, int startIndex, int count)
        {
            return _buffer.Slice(startIndex, count).IndexOf(value);
        }

        /// <summary>
        /// Inserts a single character at the specified index in the buffer, shifting subsequent characters to the right.
        /// </summary>
        /// <param name="index">The zero-based index where the character will be inserted.</param>
        /// <param name="c">The character to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="index"/> is less than 0 or greater than the current length of the buffer.
        /// </exception>
        [DebuggerStepThrough]
        public void Insert(int index, char c)
        {
            this.Insert(index, c, 1);
        }
        /// <summary>
        /// Inserts a character at the specified index in the buffer, shifting subsequent characters to the right.
        /// </summary>
        /// <param name="index">The zero-based index where the character will be inserted.</param>
        /// <param name="c">The character to insert.</param>
        /// <param name="count">The number of times to insert the character.</param>
        public void Insert(int index, char c, int count)
        {
            this.EnsureCapacity(count);
            int remaining = _position - index;

            _buffer.Slice(index, remaining).CopyTo(_buffer.Slice(index + count));
            _buffer.Slice(index, count).Fill(c);

            _position += count;
        }
        /// <summary>
        /// Inserts a span of characters at the specified index in the buffer, shifting subsequent characters to the right.
        /// </summary>
        /// <param name="index">The zero-based index where the characters will be inserted.</param>
        /// <param name="value">The span of characters to insert.</param>
        public void Insert(int index, ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return;
            }

            this.EnsureCapacity(value.Length);
            int remaining = _position - index;
            _buffer.Slice(index, remaining).CopyTo(_buffer.Slice(index + value.Length));
            value.CopyTo(_buffer.Slice(index));

            _position += value.Length;
        }

        /// <summary>
        /// Removes the specified range of characters from the buffer.
        /// </summary>
        /// <param name="startIndex">The zero-based index at which to start removing characters.</param>
        /// <param name="length">The number of characters to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void Remove(int startIndex, int length)
        {
            int position = _position;
            int newLength = startIndex + length;
            // Validate parameters
            Guard.ThrowIfNegative(length, nameof(length));
            Guard.ThrowIfNegativeOrGreaterThanOrEqual(startIndex, (uint)position, nameof(startIndex));
            Guard.ThrowIfGreaterThan(newLength, position, paramName: "'startIndex' and 'length'");

            // Calculate the number of elements to move
            int moveCount = position - newLength;

            // Shift elements to the left; CopyTo handles overlapping memory regions correctly
            if (moveCount > 0)
            {
                _buffer.Slice(newLength, moveCount).CopyTo(_buffer.Slice(startIndex));
            }

            // Update position
            _position -= length;
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
            if (_position == 0)
                return string.Empty;

            string s = new(_buffer.Slice(0, _position));
            return s;
        }

        /// <summary>
        /// Ensures that the buffer has enough capacity to accommodate the specified number of additional characters.
        /// </summary>
        /// <param name="appendLength">The number of characters to accommodate.</param>
        /// <exception cref="OutOfMemoryException">Thrown when the buffer cannot be grown further.</exception>
        private void EnsureCapacity(int appendLength)
        {
            int calculatedLength = _position + appendLength;
            if ((uint)calculatedLength > (uint)this.Capacity)
            {
                this.Grow(calculatedLength);
            }
        }
        /// <summary>
        /// Increases the capacity of the buffer to accommodate the specified minimum capacity.
        /// </summary>
        /// <param name="minimumCapacity">The minimum capacity that the buffer should have after growth.</param>
        private void Grow(int minimumCapacity)
        {
            // Ensure the capacity is a power of 2 and at least as large as minimumCapacity
            int newCapacity = Math.Max(this.Capacity * 2, minimumCapacity);
            newCapacity = BitHelper.RoundUpToPowerOf2Unsafe(newCapacity);

            Debug.Assert(newCapacity > this.Capacity, "The new capacity is smaller or equal to the current capacity.");
            Debug.Assert((newCapacity & newCapacity - 1) == 0, "The new capacity is not a power of 2."); // Ensure it's a power of 2

            _buffer.Resize(newCapacity);
        }
    }
}
