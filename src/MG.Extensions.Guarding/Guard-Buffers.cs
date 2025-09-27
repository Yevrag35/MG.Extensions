using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

#nullable enable

namespace MG.Extensions.Guarding
{
    public static partial class Guard
    {
        private const string BUFFER_TOO_SMALL = "The buffer is too small to be used as a copy destination. Expected minimum length: {0}, Received length: {1}";

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with a message that indicates that the buffer is too small with the 
        /// optional parameter name and an format provider.
        /// </summary>
        /// <param name="requiredMinimum">
        /// The minimum required length/size the buffer must be.
        /// </param>
        /// <param name="buffer">The buffer to check for sufficient size.</param>
        /// <param name="paramName">The optional parameter name of the buffer that is too small.</param>
        /// <param name="provider">The format provider to use when formatting the message.</param>
        /// <exception cref="ArgumentException"><paramref name="buffer"/> is too small for the operation to be performed.</exception>
        public static void BufferIsNotTooSmall(int requiredMinimum, Array buffer, IFormatProvider? provider = null,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(buffer))]
#endif
            string? paramName = null)
        {
            if ((uint)requiredMinimum > (uint)buffer.Length)
            {
                string message = string.Format(
                    provider: provider,
                    format: BUFFER_TOO_SMALL,
                    arg0: requiredMinimum,
                    arg1: buffer.Length);

                throw new ArgumentException(message, paramName ?? nameof(buffer));
            }
        }
    }
}
