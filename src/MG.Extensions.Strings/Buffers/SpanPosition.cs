using MG.Extensions.Guarding;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Buffers
{
    /// <summary>
    /// A read-only struct that represents a position and length range within a given span.
    /// </summary>
    [DebuggerStepThrough]
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{Index={Index}, Length={Length}\}")]
    public readonly struct SpanPosition : IComparable<int>, IComparable<SpanPosition>, IEquatable<int>, IEquatable<SpanPosition>
    {
        /// <summary>
        /// The starting index of the segment within a given span.
        /// </summary>
        public readonly int Index;
        /// <summary>
        /// The length of the segment within a given span.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Initializes a new instance of <see cref="SpanPosition"/> with the specified starting index and length.
        /// </summary>
        /// <param name="start">The zero-based starting index of the segment.</param>
        /// <param name="length">The length of the segment.</param>
        public SpanPosition(int start, int length)
        {
#if NET6_0_OR_GREATER
            Guard.ThrowIfNegative(start);
            Guard.ThrowIfNegative(length);
#else
            Guard.ThrowIfNegative(start, nameof(start));
            Guard.ThrowIfNegative(length, nameof(length));
#endif
            Index = start;
            Length = length;
        }

        /// <summary>
        /// Compares the current instance to the specified integer index.
        /// </summary>
        /// <param name="other">The zero-based index to compare to.</param>
        /// <returns>A value indicating the relative order of the objects being compared.</returns>
        public readonly int CompareTo(int other)
        {
            return Index.CompareTo(other);
        }
        /// <summary>
        /// Compares the current instance to the specified <see cref="SpanPosition"/>.
        /// </summary>
        /// <param name="other">The <see cref="SpanPosition"/> to compare to.</param>
        /// <returns>A value indicating the relative order of the objects being compared.</returns>
        public readonly int CompareTo(SpanPosition other)
        {
            return this.CompareTo(other.Index);
        }
        /// <summary>
        /// Determines whether the current instance is equal to the specified integer index.
        /// </summary>
        /// <param name="other">The zero-based index to compare against.</param>
        /// <returns>
        /// <see langword="true"/> if the current instance is equal to the specified index;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool Equals(int other)
        {
            return Index == other;
        }
        /// <summary>
        /// Determines whether the current instance is equal to the specified <see cref="SpanPosition"/>.
        /// </summary>
        /// <param name="other">The <see cref="SpanPosition"/> to compare against.</param>
        /// <returns>
        /// <see langword="true"/> if the current instance is equal to the specified <see cref="SpanPosition"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool Equals(SpanPosition other)
        {
            return Index == other.Index;
        }
        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return (obj is SpanPosition other && this.Equals(other))
                   ||
                   (obj is int i && this.Equals(i));
        }
        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return Index.GetHashCode();
        }

        /// <summary>
        /// Shifts the current <see cref="SpanPosition"/> to the left by the specified length.
        /// </summary>
        /// <param name="length">The number of indexes to shift to the left.</param>
        /// <returns>A new <see cref="SpanPosition"/> that represents the shifted position.</returns>
        public readonly SpanPosition ShiftLeft(int length)
        {
#if NET6_0_OR_GREATER
            Guard.ThrowIfNegative(length);
#else
            Guard.ThrowIfNegative(length, nameof(length));
#endif
            return new(Index - length - 1, Length);
        }

        /// <summary>
        /// Converts the specified integer index to a <see cref="SpanPosition"/>.
        /// </summary>
        /// <param name="start">The zero-based index to convert.</param>
        public static implicit operator SpanPosition(int start)
        {
            return new(start, 0);
        }

        /// <inheritdoc/>
        public static bool operator ==(SpanPosition left, SpanPosition right)
        {
            return left.Equals(right);
        }
        /// <inheritdoc/>
        public static bool operator !=(SpanPosition left, SpanPosition right)
        {
            return !left.Equals(right);
        }
    }
}
