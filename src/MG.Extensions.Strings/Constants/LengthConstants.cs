namespace MG.Extensions.Strings
{
    /// <summary>
    /// A <see langword="static"/> class containing constants for the maximum character lengths of various 
    /// numerical types.
    /// </summary>
    public static class LengthConstants
    {
        // NUMERICAL LENGTHS
        /// <summary>
        /// Maximum character length of a <see cref="byte"/>.
        /// </summary>
        public const int BYTE_MAX = 3;
        /// <summary>
        /// Maximum character length of a <see cref="short"/>.
        /// </summary>
        public const int SHORT_MAX = 6;
        /// <summary>
        /// Maximum character length of a <see cref="int"/>.
        /// </summary>
        public const int INT_MAX = 11;
        /// <summary>
        /// Maximum character length of a <see cref="long"/>.
        /// </summary>
        public const int LONG_MAX = 20;
        /// <summary>
        /// Maximum character length of a <see cref="uint"/>.
        /// </summary>
        public const int UINT_MAX = INT_MAX - 1;
        /// <summary>
        /// Maximum character length of a <see cref="ulong"/>.
        /// </summary>
        public const int ULONG_MAX = LONG_MAX;
        /// <summary>
        /// Maximum character length of a <see cref="double"/>.
        /// </summary>
        public const int DOUBLE_MAX = 24;
        /// <summary>
        /// Maximum character length of a <see cref="decimal"/>.
        /// </summary>
        public const int DECIMAL_MAX = 30;
#if NET7_0_OR_GREATER
        /// <summary>
        /// Maximum character length of an <see cref="Int128"/>.
        /// </summary>
        public const int INT128_MAX = 40;
#endif

        /// <summary>
        /// The maximum number of digits in a BigInteger struct.
        /// </summary>
        /// <remarks>
        /// The largest big integer is <c>2^68,685,922,272</c>. The following math can show
        /// the number of digits in this number:
        /// <para>
        /// The value of <c>Math.Log(2)</c> is approximately <c>0.30103</c>.
        /// </para>
        /// <para>
        /// Number of digits is roughly <c>[68,685,922,272×0.30103]+1</c>
        /// </para>
        /// <para>
        /// Number of digits is roughly <c>[20,656,128,818.57]+1</c>
        /// </para>
        /// <para>
        /// Number of digits is roughly <c>20,656,128,819</c>
        /// </para>
        /// </remarks>
        public const long BIG_INT_MAX = 20_656_128_819;
        /// <summary>
        /// Maximum character length of a <see cref="float"/>.
        /// </summary>
        /// <remarks>
        /// Equivalent to:
        /// <para><c>float.MinValue.ToString("N").Length</c></para> ...which includes 2 decimal places.
        /// </remarks>
        public const int FLOAT_MAX = 55;

        // GUID LENGTHS
        /// <summary>
        /// Maximum character length of a <see cref="Guid"/> in the "B" or "P" formats.
        /// </summary>
        public const int GUID_FORM_B_OR_P = 38;
        /// <summary>
        /// Maximum character length of a <see cref="Guid"/> in the "D" format.
        /// </summary>
        public const int GUID_FORM_N = 32;
        /// <summary>
        /// Maximum character length of a <see cref="Guid"/> in the "D" format.
        /// </summary>
        public const int GUID_FORM_D = 36;
        /// <summary>
        /// Maximum character length of a <see cref="Guid"/> in the "X" format.
        /// </summary>
        public const int GUID_FORM_X = 68;
    }
}
