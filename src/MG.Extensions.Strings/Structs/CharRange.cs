using MG.Extensions.Guarding;

#pragma warning disable IDE0009 // Member access should be qualified.
namespace MG.Extensions.Strings
{
    [DebuggerStepThrough]
    [DebuggerDisplay(@"[{Start}..{End}]")]
    public ref struct CharRange
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private char _start;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private char _end;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _length;

        public readonly int Length => _length;
        public char Start
        {
            readonly get => _start;
            set
            {
                Guard.ThrowIfGreaterThan(value, _end, nameof(Start));

                if (value == _start)
                {
                    return;
                }

                _start = value;
                _length = GetLength(in value, in _end);
            }
        }
        public char End
        {
            readonly get => _end;
            set
            {
                Guard.ThrowIfLessThan(value, _start, nameof(End));
                if (value == _end)
                {
                    return;
                }

                _end = value;
                _length = GetLength(in _start, in value);
            }
        }

        /// <inheritdoc cref="ValidateRange(in char, in char)" path="/exception"/>
        public CharRange(char start, char end)
        {
            ValidateRange(in start, in end);
            _start = start;
            _end = end;
            _length = GetLength(in start, in end);
        }

        public readonly void CopyTo(
#if NET7_0_OR_GREATER
            scoped 
#endif
            Span<char> span)
        {
            int length = this.Length;
            Guard.ThrowIfLessThan(span.Length, length, nameof(span));

            int start = this.Start;

            for (int i = 0; i < length; i++)
            {
                span[i] = (char)(start + i);
            }
        }
        public readonly void CopyTo(
#if NET7_0_OR_GREATER
            scoped 
#endif
            Span<char> span, ref int index)
        {
            this.CopyTo(span.Slice(index));
            index += this.Length;
        }
        private static int GetLength(in char start, in char end)
        {
            return end - start + 1;
        }
        public readonly char[] ToArray()
        {
            char[] array = new char[this.Length];
            this.CopyTo(array);
            return array;
        }

        /// <exception cref="ArgumentOutOfRangeException"/>
        private static void ValidateRange(in char start, in char end)
        {
#if NET6_0_OR_GREATER
            Guard.ThrowIfGreaterThan(start, end);
#else
            Guard.ThrowIfGreaterThan(start, end, nameof(start));
#endif
        }

        public static implicit operator CharRange(Span<char> span)
        {
            Guard.ThrowIfLessThan(span.Length, 2, nameof(span));
            char start = span[0];
            char end = span[span.Length - 1];

            return end < start ? new CharRange(end, start) : new CharRange(start, end);
        }
    }
}
#pragma warning restore IDE0009 // Member access should be qualified.