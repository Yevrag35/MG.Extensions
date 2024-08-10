using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace MG.Extensions.Guarding
{
    /// <summary>
    /// Provides static methods to enforce various preconditions, such as type safety, non-null values, and range 
    /// validation.
    /// </summary>
    /// <remarks>
    /// This <see langword="static"/> class acts as a safeguard, throwing exceptions when preconditions for method 
    /// arguments are not met.
    /// </remarks>
    public static partial class Guard
    {
        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if an object has been disposed.
        /// </summary>
        /// <typeparam name="T">The type of the disposed object.</typeparam>
        /// <param name="disposed">Boolean indicating whether the object is disposed.</param>
        /// <param name="instance">The instance of the object being checked.</param>
        /// <param name="objectName">The optional name of the object being checked.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the object is determined to be disposed.</exception>
        public static void ThrowIfDisposed<T>([DoesNotReturnIf(true)] bool disposed, T instance, string? objectName = null) where T : notnull
        {
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(disposed, instance);
#else
            if (disposed)
            {
                objectName ??= typeof(T).Name;
                throw new ObjectDisposedException(objectName);
            }
#endif
        }

        /// <summary>
        /// Ensures an object is of a specified type and throws an <see cref="ArgumentException"/> if not.
        /// </summary>
        /// <typeparam name="T">The expected type of the object.</typeparam>
        /// <param name="value">The object to check.</param>
        /// <param name="paramName">The name of the parameter that holds the value.</param>
        /// <returns>The object cast to the specified type.</returns>
        /// <exception cref="ArgumentException">Thrown if the value is not of the expected type.</exception>
        [return: NotNull]
        public static T ThrowIfNotType<T>([NotNull] object? value,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
            string? paramName = null)
        {
            if (!(value is T tVal))
            {
                GetTypeNames<T>(value, out string typeName, out string objTypeName);

                paramName ??= nameof(value);
                var castEx = new InvalidCastException($"The value of type \"{objTypeName}\" is not of the expected type \"{typeName}\"");
                throw new ArgumentException($"Parameter is not of the expected type \"{typeName}\".", paramName, castEx);
            }

            return tVal;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if an object is <see langword="null"/>.
        /// </summary>
        /// <param name="value">The object to check for nullity.</param>
        /// <param name="paramName">The optional name of the parameter that holds the value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static void ThrowIfNull([NotNull] object? value,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
        string? paramName = null
        )
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(value, paramName);
#else
            if (value is null)
            {
                paramName ??= nameof(value);
                throw new ArgumentNullException(paramName);
            }
#endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="value"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"><paramref name="value"/> is empty.</exception>
        /// <inheritdoc cref="ThrowIfNull(object, string)" path="/exception"/>
        public static void ThrowIfNullOrEmpty([NotNull] string? value,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
            string? paramName = null)
        {
#if NET7_0_OR_GREATER
            ArgumentException.ThrowIfNullOrEmpty(value, paramName);
            return;
#else
            ThrowIfNull(value, paramName);
            if (string.Empty == value)
            {
                paramName ??= nameof(value);
                throw new ArgumentException("The specified string value is empty.", paramName);
            }
#endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if a string is <see langword="null"/>, empty, or consists 
        /// only of white-space characters.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="paramName">The optional name of the parameter that holds the value.</param>
        /// <exception cref="ArgumentException">Thrown if the string is null, empty, or only white space.</exception>
        /// <inheritdoc cref="ThrowIfNull(object, string)" path="/exception"/>
        public static void ThrowIfNullOrWhitespace([NotNull] string? value,
#if NET6_0_OR_GREATER
            [CallerArgumentExpression(nameof(value))]
#endif
            string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
            return;
#else
            ThrowIfNullOrEmpty(value, paramName);
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    return;
                }
            }

            paramName ??= nameof(value);
            throw new ArgumentException("The specified string value is whitespace.", paramName);
#endif
        }

        private static void GetTypeNames<T>(object? value, out string typeName, out string objTypeName)
        {
            Type t = typeof(T);
            Type? oType = value?.GetType();

            typeName = t.FullName ?? t.Name;
            objTypeName = oType is null
                ? "null"
                : oType.FullName ?? oType.Name;
        }
    }
}
