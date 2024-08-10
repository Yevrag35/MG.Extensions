#if NET6_0_OR_GREATER

using System.Numerics;

namespace MG.Extensions.Strings.Builders
{
    /// <summary>
    /// Encapsulates a method that receives a span of <see cref="char"/> objects and a state object of 
    /// type <typeparamref name="T"/> returning the number of characters copied to the span.
    /// </summary>
    /// <typeparam name="T">The type of the object that represents the state.</typeparam>
    /// <param name="span">A span of <see cref="char"/> objects.</param>
    /// <param name="state">A state object of type <typeparamref name="T"/>.</param>
    /// <returns>
    /// The number of characters written to <paramref name="span"/>.
    /// </returns>
    public delegate int WriteToSpan<T>(Span<char> span, T state);

    public ref partial struct SpanStringBuilder
    {
#if NET7_0_OR_GREATER
        /// <summary>
        /// Appends the string representation of a specified <typeparamref name="T"/> value to this instance.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="INumber{TSelf}"/> being appended.</typeparam>
        /// <param name="number">The number to append.</param>
        /// <param name="format">The format to use.</param>
        /// <param name="provider">The format provider to use.</param>
        /// <returns>
        /// <inheritdoc cref="Append(char)"/>
        /// </returns>
        public SpanStringBuilder Append<T>(T number, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : INumber<T>, IMinMaxValue<T>
        {
            int length = number.GetLength();
            this.EnsureCapacity(length);
            number.CopyToSlice(_span, ref _position, format, provider);

            return this;
        }
#endif
        /// <summary>
        /// Appends the string representation of a specified <typeparamref name="T"/> value to this instance.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="ISpanFormattable"/> being appended.</typeparam>
        /// <param name="formattable">The formattable value to append.</param>
        /// <param name="maxLength">The maximum length of the value to append.</param>
        /// <param name="format">The format to use.</param>
        /// <param name="provider">The format provider to use.</param>
        /// <returns>
        /// <inheritdoc cref="Append(char)"/>
        /// </returns>
        /// <inheritdoc cref="EnsureCapacity(int)" path="/exception"/>
        public SpanStringBuilder Append<T>(T formattable, int maxLength,
#if NET7_0_OR_GREATER
            scoped
#endif
            ReadOnlySpan<char> format = default,
            IFormatProvider? provider = null) where T : ISpanFormattable
        {
            this.EnsureCapacity(maxLength);
            formattable.CopyToSlice(_span, ref _position, format, provider);

            return this;
        }

        /// <summary>
        /// Appends characters to the current instance using the specified callback delegate.
        /// </summary>
        /// <typeparam name="T">The type of state provided to the delegate.</typeparam>
        /// <param name="length">
        /// The total, exact number of <see cref="char"/> that will be written to the span.
        /// </param>
        /// <param name="state">
        /// The state provided to the delegate preventing the need for a closure.
        /// </param>
        /// <param name="spanAction">
        /// The action to perform on the span.
        /// </param>
        /// <returns>
        /// This same <see cref="SpanStringBuilder"/> instance for chaining.
        /// </returns>
        public SpanStringBuilder Append<T>(int length, T state, SpanAction<char, T> spanAction)
        {
            this.EnsureCapacity(length);
            spanAction(_span.Slice(_position, length), state);
            _position += length;

            return this;
        }

        /// <summary>
        /// Appends characters to the current instance using the specified callback delegate.
        /// </summary>
        /// <typeparam name="T">The type of the object that represents the state.</typeparam>
        /// <param name="maxLength">
        /// The maximum number of <see cref="char"/> that will be written to the span.
        /// </param>
        /// <param name="state">
        /// A state object of type <typeparamref name="T"/>.
        /// </param>
        /// <param name="spanFunc">
        /// The function to perform on the span returning the number of characters that were written.
        /// </param>
        /// <returns>
        /// This same <see cref="SpanStringBuilder"/> instance for chaining.
        /// </returns>
        public SpanStringBuilder Append<T>(int maxLength, T state, WriteToSpan<T> spanFunc)
        {
            this.EnsureCapacity(maxLength);
            int written = spanFunc(_span.Slice(_position, maxLength), state);
            _position += written;

            return this;
        }
    }
}
#endif