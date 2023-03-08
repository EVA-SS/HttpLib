using System.Globalization;

namespace System.Web.Http
{
    /// <summary>
    /// Utility class for creating and unwrapping <see cref="T:System.Exception" /> instances.
    /// </summary>
    internal static class Error
    {
        /// <summary>
        /// Formats the specified resource string using <see cref="M:CultureInfo.CurrentCulture" />.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The formatted string.</returns>
        internal static string Format(string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        /// Creates an <see cref="T:System.ArgumentException" /> with the provided properties.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <param name="messageFormat">A composite format string explaining the reason for the exception.</param>
        /// <param name="messageArgs">An object array that contains zero or more objects to format.</param>
        /// <returns>The logged <see cref="T:System.Exception" />.</returns>
        internal static ArgumentException Argument(string parameterName, string messageFormat, params object[] messageArgs)
        {
            return new ArgumentException(Format(messageFormat, messageArgs), parameterName);
        }

        /// <summary>
        /// Creates an <see cref="T:System.ArgumentNullException" /> with the provided properties.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <returns>The logged <see cref="T:System.Exception" />.</returns>
        internal static ArgumentNullException ArgumentNull(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Creates an <see cref="T:System.InvalidOperationException" />.
        /// </summary>
        /// <param name="messageFormat">A composite format string explaining the reason for the exception.</param>
        /// <param name="messageArgs">An object array that contains zero or more objects to format.</param>
        /// <returns>The logged <see cref="T:System.Exception" />.</returns>
        internal static InvalidOperationException InvalidOperation(string messageFormat, params object[] messageArgs)
        {
            return new InvalidOperationException(Format(messageFormat, messageArgs));
        }
    }
}
