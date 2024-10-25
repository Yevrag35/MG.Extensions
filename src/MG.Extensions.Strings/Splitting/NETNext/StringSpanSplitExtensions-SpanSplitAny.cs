using MG.Extensions.Strings.Enumerators;

namespace MG.Extensions.Strings
{
    public static partial class StringSpanSplitExtensions
    {
        public static SplitAnyEnumerator SpanSplitAny(this ReadOnlySpan<char> readOnlySpan, SearchValues<char> searchValues)
        {
            return new SplitAnyEnumerator(readOnlySpan, searchValues);
        }
        [DebuggerStepThrough]
        public static SplitAnyEnumerator SpanSplitAny(this Span<char> span, SearchValues<char> searchValues)
        {
            return SpanSplitAny(readOnlySpan: span, searchValues);
        }
        [DebuggerStepThrough]
        public static SplitAnyEnumerator SpanSplitAny(this string? value, SearchValues<char> searchValues)
        {
            return SpanSplitAny(readOnlySpan: value.AsSpan(), searchValues);
        }

#if NET9_0_OR_GREATER
        public static SplitAnyStringEnumerator SpanSplitAny(this ReadOnlySpan<char> readOnlySpan, SearchValues<string> searchValues)
        {
            return new SplitAnyStringEnumerator(readOnlySpan, searchValues);
        }
        [DebuggerStepThrough]
        public static SplitAnyStringEnumerator SpanSplitAny(this Span<char> span, SearchValues<string> searchValues)
        {
            return SpanSplitAny(readOnlySpan: span, searchValues);
        }
        [DebuggerStepThrough]
        public static SplitAnyStringEnumerator SpanSplitAny(this string? value, SearchValues<string> searchValues)
        {
            return SpanSplitAny(readOnlySpan: value.AsSpan(), searchValues);
        }
#endif
    }
}