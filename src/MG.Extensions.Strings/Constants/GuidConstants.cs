namespace MG.Extensions.Strings.Constants
{
    /// <summary>
    /// A <see langword="static"/> constant class for the format strings <see cref="Guid"/> structs.
    /// </summary>
    public static class GuidConstants
    {
        /// <summary>
        /// The format string for the <see cref="Guid"/> struct in the "B" format.
        /// </summary>
        /// <remarks>
        /// 32 digits separated by hyphens, enclosed in braces:
        /// <para><c>{00000000-0000-0000-0000-000000000000}</c></para>
        /// </remarks>
        public const string B_FORMAT = "B";
        /// <summary>
        /// The format string for the <see cref="Guid"/> struct in the "D" format.
        /// </summary>
        /// <remarks>
        /// <para>The defualt format when none is specified.</para>
        /// 32 digits separated by hyphens:
        /// <para><c>00000000-0000-0000-0000-000000000000</c></para>
        /// </remarks>
        public const string D_FORMAT = "D";
        /// <summary>
        /// The format string for the <see cref="Guid"/> struct in the "N" format.
        /// </summary>
        /// <remarks>
        /// <para>
        /// 32 digits:
        /// <para>
        /// <c>00000000000000000000000000000000</c>
        /// </para>
        /// </para>
        /// </remarks>
        public const string N_FORMAT = "N";
        /// <summary>
        /// The format string for the <see cref="Guid"/> struct in the "P" format.
        /// </summary>
        /// <remarks>
        /// 32 digits separated by hyphens, enclosed in parentheses:
        /// <para>
        /// <c>(00000000-0000-0000-0000-000000000000)</c>
        /// </para>
        /// </remarks>
        public const string P_FORMAT = "P";
        /// <summary>
        /// The format string for the <see cref="Guid"/> struct in the "X" format.
        /// </summary>
        /// <remarks>
        /// Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight 
        /// hexadecimal values that is also enclosed in braces:
        /// <para>
        /// <c>{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}</c>
        /// </para>
        /// </remarks>
        public const string X_FORMAT = "X";
    }
}
