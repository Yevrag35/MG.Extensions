using System.Runtime.InteropServices;

namespace MG.Extensions.Strings
{
    /// <summary>
    /// A ref struct that keeps track of the number of <see langword="true"/> boolean values that have been set.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public ref struct BoolCounter
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _count;
        private readonly Span<bool> _counted;

        /// <summary>
        /// The number of <see langword="true"/> boolean values that have been set.
        /// </summary>
        public readonly int Count => _count;

        /// <summary>
        /// Initializes a new instance of <see cref="BoolCounter"/> using the specified <see cref="Span{T}"/> of 
        /// <see cref="bool"/> as its internal buffer.
        /// </summary>
        /// <param name="counted">The passed span to use as the internal buffer.</param>
        public BoolCounter(Span<bool> counted)
        {
            _counted = counted;
            _count = 0;
        }

        private readonly bool IndexHasFlag(in int flag)
        {
            return _counted[flag];
        }

        /// <summary>
        /// Attempts to set the flag at the specified index to <see langword="true"/>.
        /// </summary>
        /// <param name="flagIndex">The flag index to mark.</param>
        /// <param name="value"></param>
        /// <returns>
        /// <see langword="true"/> if the flag was set; otherwise,
        /// if <paramref name="value"/> is <see langword="false"/> or the flag was already set, 
        /// <see langword="false"/>.
        /// </returns>
        public bool MarkFlag(int flagIndex, bool value)
        {
            return value && this.MarkFlag(in flagIndex);
        }
        /// <summary>
        /// Attempts to set the flag at the specified index to <see langword="true"/>.
        /// </summary>
        /// <param name="flagIndex">
        /// The flag index to mark.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the flag was set; otherwise,
        /// if the flag was already set, <see langword="false"/>.
        /// </returns>
        public bool MarkFlag(in int flagIndex)
        {
            bool result = false;

            if (!this.IndexHasFlag(in flagIndex))
            {
                result = true;
                _counted[flagIndex] = result;
                _count++;
            }

            return result;
        }

        /// <summary>
        /// Indicates whether any enumerators should continue to the next element.
        /// </summary>
        /// <remarks>
        /// When this instance returns <see langword="false"/>, this indicates that all flags have been set,
        /// and that any enumerators should stop and return.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if the enumerator should continue; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public readonly bool MoveNext()
        {
            return _count < _counted.Length;
        }
    }
}
