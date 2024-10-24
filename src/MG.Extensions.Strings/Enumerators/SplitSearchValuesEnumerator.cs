#if NET8_0_OR_GREATER

using System.Collections;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Enumerators
{
    [StructLayout(LayoutKind.Auto)]
    public ref struct SplitAnyEnumerator
    {
        private ReadOnlySpan<char> _original;
        private SearchValues<char> _splitBy;
        private ReadOnlySpan<char> _text;
        private SplitSearchValuesEntry<char> _current;

        public readonly SplitSearchValuesEntry<T> Current => _current;
        public readonly SearchValues<char> SplitBy => _splitBy;

        public SplitAnyEnumerator(ReadOnlySpan<char> text, SearchValues<char> splitBy)
        {
            _text = text;
            _original = text;
            _splitBy = splitBy;
            _current = SplitSearchValuesEntry.EmptyCharEntry;
        }

        public readonly SplitAnyEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            ReadOnlySpan<char> chars = _text;

            if (chars.IsEmpty)
            {
                return false;
            }

            int index = chars.IndexOfAny(this.SplitBy);
            if (index == -1)
            {
                _text = [];
                _current = new SplitSearchValuesEntry<char>(chars, this.SplitBy);
                return true;
            }

            _current = new(chars.Slice(0, index), this.SplitBy);
            _text = (uint)index + 1u >= (uint)chars.Length ? [] : chars.Slice(index + 1);
            return true;
        }

        public void Reset()
        {
            _text = _original;
            _current = SplitSearchValuesEntry.EmptyCharEntry;
        }
    }

#if NET9_0_OR_GREATER
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay(@"\{Current = {Current}\}")]
    public ref struct SplitAnyStringEnumerator : IEnumerator<SplitSearchValuesEntry<string>>
    {
        private ReadOnlySpan<char> _original;
        private SearchValues<string>? _splitBy;
        private ReadOnlySpan<char> _text;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SplitSearchValuesEntry<string> _current;

        public readonly SplitSearchValuesEntry<string> Current => _current;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly object IEnumerator.Current => _current.Section.ToString();
        public readonly SearchValues<string> SplitBy => _splitBy ?? SplitSearchValuesEntry.GetEmptyStrings();

        public SplitAnyStringEnumerator(ReadOnlySpan<char> text, SearchValues<string> splitBy)
        {
            _original = text;
            _text = text;
            _splitBy = splitBy;
            _current = SplitSearchValuesEntry.EmptyStringEntry;
        }

        public void Dispose()
        {
            this = default;
        }

        public readonly SplitAnyStringEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            ReadOnlySpan<char> chars = _text;

            if (chars.IsEmpty)
            {
                return false;
            }

            int index = chars.IndexOfAny(this.SplitBy);
            if (index == -1)
            {
                _text = [];
                _current = new SplitSearchValuesEntry<string>(chars, this.SplitBy);
                return true;
            }

            _current = new(chars.Slice(0, index), this.SplitBy);
            _text = (uint)index + 1u >= (uint)chars.Length ? [] : chars.Slice(index + 1);
            return true;
        }

        public void Reset()
        {
            _text = _original;
            _current = SplitSearchValuesEntry.EmptyStringEntry;
        }
    }
#endif
}
#endif