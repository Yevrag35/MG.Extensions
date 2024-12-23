using MG.Extensions.Guarding;
using System.Reflection;

namespace MG.Extensions.Strings.Reflection
{
    /// <summary>
    /// An extension class for <see cref="Type"/> objects that provides functionality for 
    /// obtaining the name of the type.
    /// </summary>
    public static class TypeNameExtensions
    {
        /// <summary>
        /// Returns the fully qualified name or the member name of the current <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type whose name will be returned.</param>
        /// <returns>
        /// The <see cref="Type.FullName"/> of the current <see cref="Type"/> if it is not <see langword="null"/>;
        /// otherwise, the <see cref="MemberInfo.Name"/> instead.  If the type being extended is <see langword="null"/>,
        /// then an empty string is returned.
        /// </returns>
        public static string GetName(this Type type)
        {
            Guard.ThrowIfNull(type, nameof(type));
            return type.FullName ?? type.Name;
        }

        /// <summary>
        /// Returns the fully qualified name or the member name of the current <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type whose name will be returned.</param>
        /// <returns>
        /// The <see cref="Type.FullName"/> of the current <see cref="Type"/> if it is not <see langword="null"/>;
        /// otherwise, the <see cref="MemberInfo.Name"/> instead.  If the type being extended is <see langword="null"/>,
        /// then an empty string is returned.
        /// </returns>
        [return: NotNullIfNotNull(nameof(type))]
        public static string? GetNameOrNull(this Type? type)
        {
            if (type is null)
            {
                return null;
            }

            return type.FullName ?? type.Name;
        }
    }
}
