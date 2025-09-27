#if NETSTANDARD2_0
using MG.Extensions.Strings.Constants;
using System.Runtime.InteropServices;

namespace MG.Extensions.Strings.Unmanaged
{
    /// <summary>
    /// Allocation-free formatting helpers for <see cref="Guid"/> compatible with .NET Standard 2.0.
    /// </summary>
    /// <remarks>
    /// Supports the standard format specifiers <c>"D"</c>, <c>"N"</c>, <c>"B"</c>, <c>"P"</c>, and <c>"X"</c>.
    /// Behavior mirrors <see cref="Guid.ToString(string)"/> without creating a string by writing into a <see cref="Span{T}"/>.
    /// </remarks>
    internal static class GuidFormatting
    {
        /// <summary>
        /// Attempts to write <paramref name="value"/> into <paramref name="destination"/> using the specified <paramref name="format"/>.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> to format.</param>
        /// <param name="destination">The destination <see cref="Span{T}"/> of <see langword="char"/> to receive the characters.</param>
        /// <param name="charsWritten">On success, receives the number of characters written.</param>
        /// <param name="format">
        /// A <see cref="ReadOnlySpan{T}"/> of <see langword="char"/> with one of: <c>"D"</c>, <c>"N"</c>, <c>"B"</c>, <c>"P"</c>, <c>"X"</c>.
        /// If empty, defaults to <c>"D"</c>.
        /// </param>
        /// <returns><see langword="true"/> if formatting succeeded; otherwise <see langword="false"/> (usually due to insufficient destination length).</returns>
        public static bool TryFormat(Guid value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format)
        {
            char f = (format.Length == 0) ? GuidConstants.D_FORMAT[0] : (char)ToUpperInvariant(format[0]);
            int required = RequiredLength(f);
            if (required == 0 || destination.Length < required)
            {
                charsWritten = 0;
                return false;
            }

            // Reinterpret Guid layout without allocating.
            ref readonly GuidLayout g = ref Unsafe.As<Guid, GuidLayout>(ref value);

            int idx = 0;
            switch (f)
            {
                case 'D':
                    // dddddddd-dddd-dddd-dddd-dddddddddddd
                    WriteHex(destination, ref idx, g.A, 8);
                    destination[idx++] = '-';
                    WriteHex(destination, ref idx, (ushort)g.B, digits: 4);
                    destination[idx++] = '-';
                    WriteHex(destination, ref idx, (ushort)g.C, digits: 4);
                    destination[idx++] = '-';
                    WriteHex(destination, ref idx, g.D);
                    WriteHex(destination, ref idx, g.E);
                    destination[idx++] = '-';
                    WriteHex(destination, ref idx, g.F);
                    WriteHex(destination, ref idx, g.G);
                    WriteHex(destination, ref idx, g.H);
                    WriteHex(destination, ref idx, g.I);
                    WriteHex(destination, ref idx, g.J);
                    WriteHex(destination, ref idx, g.K);
                    break;

                case 'N':
                    // dddddddddddddddddddddddddddddddd (no separators)
                    WriteHex(destination, ref idx, g.A, 8);
                    WriteHex(destination, ref idx, (ushort)g.B, 4);
                    WriteHex(destination, ref idx, (ushort)g.C, 4);
                    WriteHex(destination, ref idx, g.D);
                    WriteHex(destination, ref idx, g.E);
                    WriteHex(destination, ref idx, g.F);
                    WriteHex(destination, ref idx, g.G);
                    WriteHex(destination, ref idx, g.H);
                    WriteHex(destination, ref idx, g.I);
                    WriteHex(destination, ref idx, g.J);
                    WriteHex(destination, ref idx, g.K);
                    break;

                case 'B':
                    // {dddddddd-dddd-dddd-dddd-dddddddddddd}
                    destination[idx++] = '{';
                    FormatCore_D(destination, ref idx, in g);
                    destination[idx++] = '}';
                    break;

                case 'P':
                    // (dddddddd-dddd-dddd-dddd-dddddddddddd)
                    destination[idx++] = '(';
                    FormatCore_D(destination, ref idx, in g);
                    destination[idx++] = ')';
                    break;

                case 'X':
                    // {0xdddddddd,0xdddd,0xdddd,{0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd}}
                    destination[idx++] = '{';
                    destination[idx++] = '0';
                    destination[idx++] = 'x';
                    WriteHex(destination, ref idx, g.A, 8);
                    destination[idx++] = ',';
                    destination[idx++] = '0';
                    destination[idx++] = 'x';
                    WriteHex(destination, ref idx, (ushort)g.B, 4);
                    destination[idx++] = ',';
                    destination[idx++] = '0';
                    destination[idx++] = 'x';
                    WriteHex(destination, ref idx, (ushort)g.C, 4);
                    destination[idx++] = ',';
                    destination[idx++] = '{';
                    Write0xByte(destination, ref idx, g.D);
                    destination[idx++] = ',';
                    Write0xByte(destination, ref idx, g.E);
                    destination[idx++] = ',';
                    Write0xByte(destination, ref idx, g.F);
                    destination[idx++] = ',';
                    Write0xByte(destination, ref idx, g.G);
                    destination[idx++] = ',';
                    Write0xByte(destination, ref idx, g.H);
                    destination[idx++] = ',';
                    Write0xByte(destination, ref idx, g.I);
                    destination[idx++] = ',';
                    Write0xByte(destination, ref idx, g.J);
                    destination[idx++] = ',';
                    Write0xByte(destination, ref idx, g.K);
                    destination[idx++] = '}';
                    destination[idx++] = '}';
                    break;

                default:
                    charsWritten = 0;
                    return false;
            }

            charsWritten = idx;
            return true;
        }

        /// <summary>Returns the required character count for a given format specifier.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RequiredLength(char f)
        {
            f = (char)ToUpperInvariant(f);
            switch (f)
            {
                case 'N':
                    return LengthConstants.GUID_FORM_N;
                case 'D':
                    return LengthConstants.GUID_FORM_D;
                case 'B':
                case 'P':
                    return LengthConstants.GUID_FORM_B_OR_P;
                case 'X':
                    return LengthConstants.GUID_FORM_X;
                default:
                    return 0;  // unsupported
            }
        }

        // Reuse the 'D' core to avoid duplication for 'B' and 'P'
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FormatCore_D(Span<char> dest, ref int i, in GuidLayout g)
        {
            WriteHex(dest, ref i, g.A, 8);
            dest[i++] = '-';
            WriteHex(dest, ref i, (ushort)g.B, 4);
            dest[i++] = '-';
            WriteHex(dest, ref i, (ushort)g.C, 4);
            dest[i++] = '-';
            WriteHex(dest, ref i, g.D);
            WriteHex(dest, ref i, g.E);
            dest[i++] = '-';
            WriteHex(dest, ref i, g.F);
            WriteHex(dest, ref i, g.G);
            WriteHex(dest, ref i, g.H);
            WriteHex(dest, ref i, g.I);
            WriteHex(dest, ref i, g.J);
            WriteHex(dest, ref i, g.K);
        }

        // Hex helpers (uppercase to match Guid default formatting)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteHex(Span<char> dest, ref int i, int value, int digits)
        {
            // write big-endian within the field (matches Guid string formatting)
            for (int shift = (digits - 1) * 4; shift >= 0; shift -= 4)
                dest[i++] = Hex((byte)((value >> shift) & 0xF));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteHex(Span<char> dest, ref int i, ushort value, int digits)
        {
            for (int shift = (digits - 1) * 4; shift >= 0; shift -= 4)
            {
                dest[i++] = Hex((byte)((value >> shift) & 0xF));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteHex(Span<char> dest, ref int i, byte value)
        {
            dest[i++] = Hex((byte)(value >> 4));
            dest[i++] = Hex((byte)(value & 0xF));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Write0xByte(Span<char> dest, ref int i, byte value)
        {
            dest[i++] = '0';
            dest[i++] = 'x';
            WriteHex(dest, ref i, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char Hex(byte v)
        {
            return (char)(v < 10 ? ('0' + v) : ('A' + (v - 10)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ToUpperInvariant(int ch)
        {
            return (ch >= 'a' && ch <= 'z') ? (ch - 32) : ch;
        }

        // Matches the in-memory field order used by Guid's standard string formatting
        // (A, B, C are numeric fields; the rest are individual bytes).
        [StructLayout(LayoutKind.Sequential)]
        private readonly struct GuidLayout
        {
            public readonly int A;
            public readonly short B;
            public readonly short C;
            public readonly byte D, E, F, G, H, I, J, K;
        }
    }
}
#endif