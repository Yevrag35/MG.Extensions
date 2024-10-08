using MG.Extensions.Guarding;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings
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

        public readonly int CompareTo(int other)
        {
            return Index.CompareTo(other);
        }
        public readonly int CompareTo(SpanPosition other)
        {
            return this.CompareTo(other.Index);
        }
        public readonly bool Equals(int other)
        {
            return Index == other;
        }
        public readonly bool Equals(SpanPosition other)
        {
            return Index == other.Index;
        }
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return (obj is SpanPosition other && this.Equals(other))
                   ||
                   (obj is int i && this.Equals(i));
        }
        public override readonly int GetHashCode()
        {
            return Index.GetHashCode();
        }

        public readonly SpanPosition ShiftLeft(int length)
        {
            return new(Index - length - 1, Length);
        }

        public static implicit operator SpanPosition(int start)
        {
            return new(start, 0);
        }

        public static bool operator ==(SpanPosition left, SpanPosition right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(SpanPosition left, SpanPosition right)
        {
            return !left.Equals(right);
        }
    }
}
