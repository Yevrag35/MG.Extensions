using MG.Extensions.Guarding;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Buffers
{
    /// <summary>
    /// A read-only struct that represents a position and length range within a given span.
    /// </summary>
    [DebuggerStepThrough]
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay(@"\{Index={Index}, Length={Length}\}")]
    public readonly struct SpanPosition : IComparable<int>, IComparable<SpanPosition>, IEquatable<int>, IEquatable<SpanPosition>
    {
        private readonly int _index;
        private readonly int _length;

        /// <summary>
        /// The exclusive end index of the segment within a given span.
        /// </summary>
        public readonly int End => _index + _length;
        /// <summary>
        /// The starting index of the segment within a given span.
        /// </summary>
        public int Index => _index;
        /// <summary>
        /// Indicates whether the segment is defined within a given span or represents an undefined region.
        /// </summary>
        public readonly bool IsDefined => _length > 0;
        /// <summary>
        /// The length of the segment within a given span.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanPosition"/> struct with the specified start index and length.
        /// </summary>
        /// <param name="start">The starting index of the span. Must be greater than or equal to 0.</param>
        /// <param name="length">The length of the span segment. Must be non-zero positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="start"/> is less than 0 or <paramref name="length"/> is negative.</exception>
        public SpanPosition(int start, int length)
        {
            Guard.ThrowIfNegative(start, nameof(start));
            Guard.ThrowIfNegativeOrZero(length, nameof(length));
            _index = start;
            _length = length;
        }

        /// <summary>
        /// Compares the current <see cref="SpanPosition"/> to another <see cref="SpanPosition"/> object.
        /// </summary>
        /// <param name="other">The <see cref="SpanPosition"/> to compare with.</param>
        /// <returns>An integer that indicates the relative position in the sort order.</returns>
        public int CompareTo(SpanPosition other)
        {
            int comparison = _index.CompareTo(other._index);
            if (comparison == 0)
            {
                comparison = _length.CompareTo(other._length);
            }

            return comparison;
        }
        /// <summary>
        /// Compares the current object's index value to a specified integer and returns an indication of their relative
        /// values.
        /// </summary>
        /// <param name="other">The integer value to compare with the current object's index.</param>
        /// <returns>A signed integer that indicates the relative order of the index and the specified value: less than zero if
        /// the index is less than <paramref name="other"/>; zero if they are equal; greater than zero if the index is
        /// greater than <paramref name="other"/>.</returns>
        public int CompareTo(int other)
        {
            return _index.CompareTo(other);
        }
        /// <summary>
        /// Determines whether the current <see cref="SpanPosition"/> is equal to the specified integer value representing a starting index.
        /// </summary>
        /// <param name="other">The integer value to compare with.</param>
        /// <returns><see langword="true"/> if the starting index is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        public bool Equals(int other)
        {
            return _index == other;
        }
        /// <summary>
        /// Determines whether the current <see cref="SpanPosition"/> is equal to another <see cref="SpanPosition"/> object.
        /// </summary>
        /// <param name="other">The <see cref="SpanPosition"/> to compare with.</param>
        /// <returns><see langword="true"/> if both objects represent the same segment; otherwise, <see langword="false"/>.</returns>
        public bool Equals(SpanPosition other)
        {
            return _index == other._index && _length == other._length;
            ;
        }
        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj switch
            {
                SpanPosition pos => this.Equals(pos),
                int i => this.Equals(i),
                long longVal when longVal <= int.MaxValue && longVal >= int.MinValue => this.Equals((int)longVal),
                uint untInt when untInt <= int.MaxValue => this.Equals((int)untInt),
                ulong untLong when untLong <= int.MaxValue => this.Equals((int)untLong),
                _ => false,
            };
        }
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(_index, _length);
        }

        /// <summary>
        /// Converts the current <see cref="SpanPosition"/> to a <see cref="Range"/> object.
        /// </summary>
        /// <returns>A <see cref="Range"/> object representing the span position and length.</returns>
        public Range ToRange()
        {
            return this.IsDefined ? new Range(_index, this.End) : new Range(0, 0);
        }

        /// <summary>
        /// Throws an exception if the specified <see cref="SpanPosition"/> is undefined.
        /// </summary>
        /// <param name="position">The <see cref="SpanPosition"/> to validate. Must have a non-zero length.</param>
        /// <param name="paramName">The name of the parameter being validated. Automatically populated by the compiler if not explicitly provided.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="position"/> has a length of zero, indicating an undefined span position.</exception>
        public static void ThrowIfUndefined(SpanPosition position,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(position))]
#endif
            string? paramName = null)
        {
            if (0 <= position._length)
            {
                throw new ArgumentOutOfRangeException(paramName, "The span position is undefined (length is zero).");
            }
        }

        /// <summary>
        /// Represents an undefined region of a span, where the index and length are both invalid.
        /// </summary>
        public static readonly SpanPosition Undefined = default;

        /// <summary>
        /// Determines whether two <see cref="SpanPosition"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="SpanPosition"/> to compare.</param>
        /// <param name="right">The second <see cref="SpanPosition"/> to compare.</param>
        /// <returns><see langword="true"/> if both <see cref="SpanPosition"/> instances are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(SpanPosition left, SpanPosition right)
        {
            return left._index == right._index && left._length == right._length;
        }
        /// <summary>
        /// Determines whether two <see cref="SpanPosition"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="SpanPosition"/> to compare.</param>
        /// <param name="right">The second <see cref="SpanPosition"/> to compare.</param>
        /// <returns><see langword="true"/> if both <see cref="SpanPosition"/> instances are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(SpanPosition left, SpanPosition right)
        {
            return left._index != right._index || left._length != right._length;
        }
        /// <summary>
        /// Converts a <see cref="SpanPosition"/> to a <see cref="Range"/> representing the same position.
        /// </summary>
        /// <remarks>This implicit conversion allows a <see cref="SpanPosition"/> to be used wherever a
        /// <see cref="Range"/> is expected. The resulting <see cref="Range"/> will represent the position specified by
        /// <paramref name="position"/>.</remarks>
        /// <param name="position">The <see cref="SpanPosition"/> to convert to a <see cref="Range"/>.</param>
        public static implicit operator Range(SpanPosition position)
        {
            return position.ToRange();
        }
        /// <summary>
        /// Determines whether the specified left position is greater than the specified right position.
        /// </summary>
        /// <param name="left">The first <see cref="SpanPosition"/> to compare.</param>
        /// <param name="right">The second <see cref="SpanPosition"/> to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(SpanPosition left, SpanPosition right)
        {
            return left.CompareTo(right) > 0;
        }
        /// <summary>
        /// Determines whether one SpanPosition instance precedes another in a comparison operation.
        /// </summary>
        /// <remarks>This operator enables comparison of SpanPosition instances using the less-than (<)
        /// operator. The comparison is based on the ordering defined by the CompareTo method.</remarks>
        /// <param name="left">The first SpanPosition to compare.</param>
        /// <param name="right">The second SpanPosition to compare.</param>
        /// <returns>true if left is less than right; otherwise, false.</returns>
        public static bool operator <(SpanPosition left, SpanPosition right)
        {
            return left.CompareTo(right) < 0;
        }
        /// <summary>
        /// Determines whether one SpanPosition is greater than or equal to another.
        /// </summary>
        /// <param name="left">The first SpanPosition to compare.</param>
        /// <param name="right">The second SpanPosition to compare.</param>
        /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(SpanPosition left, SpanPosition right)
        {
            return left.CompareTo(right) >= 0;
        }
        /// <summary>
        /// Determines whether one SpanPosition is less than or equal to another.
        /// </summary>
        /// <param name="left">The first SpanPosition to compare.</param>
        /// <param name="right">The second SpanPosition to compare against.</param>
        /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(SpanPosition left, SpanPosition right)
        {
            return left.CompareTo(right) <= 0;
        }
    }
}
