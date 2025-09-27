using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace MG.Extensions.Guarding
{
    public static partial class Guard
    {
        //private const string NULL = "<null>";
        private const string COLLECTION_EXPECTED_ONE_OR_MORE = "The collection must contain at least one element.";
        //private const string COLLECTION_ITEM_IS_NULL = "An element in the array/span is null and was guarded against.";
        //private const string COLLECTION_NULL_VALUES_AT = "The passed collection contains illegal null values at the following indexes: {0}";

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with a message that indicates that the collection is empty when it should not be.
        /// </summary>
        /// <param name="array">The array to check for emptiness.</param>
        /// <param name="paramName">
        /// The parameter name of the collection that is empty.
        /// </param>
        /// <exception cref="ArgumentException"/>
        public static void ArrayIsNotEmpty(
            Array array,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(array))]
#endif
            string? paramName = null)
        {
            if (0 == array.Length)
            {
                throw new ArgumentException(COLLECTION_EXPECTED_ONE_OR_MORE, paramName);
            }
        }
    }
}
