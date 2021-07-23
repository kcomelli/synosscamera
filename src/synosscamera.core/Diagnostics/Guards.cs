using synosscamera.core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Diagnostics
{
    /// <summary>
    /// Diagnostic guard utilities
    /// </summary>
    public static class Guards
    {
        /// <summary>
        /// Throw if argument is null 
        /// </summary>
        /// <param name="source">Source object to check</param>
        /// <param name="name">Name of argument</param>
        public static void CheckArgumentNull(this object source, string name)
        {
            if (source == null)
                throw new ArgumentNullException(name);
        }
        /// <summary>
        /// Throw if string argument is null or empty
        /// </summary>
        /// <param name="source">Source string to check</param>
        /// <param name="name">Name of argument</param>
        public static void CheckArgumentNullOrEmpty(this string source, string name)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(name);
        }
        /// <summary>
        /// Throw if source obejct is null
        /// </summary>
        /// <param name="source">Source to check</param>
        /// <param name="message">Message for exception of null if default</param>
        public static void CheckNull(this object source, string message)
        {
            if (source == null)
#pragma warning disable S112 // General exceptions should never be thrown
                throw new NullReferenceException(message);
#pragma warning restore S112 // General exceptions should never be thrown
        }
        /// <summary>
        /// Throw if source string is null or empty with argumentException
        /// </summary>
        /// <param name="source">Source string to test</param>
        /// <param name="name">Name of argument tested</param>
        public static void CheckMandatoryOption(this string source, string name)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException(string.Format(Errors.Exception_OptionMustBeProvided, name));
        }
        /// <summary>
        /// Throw if source string is null or empty with argumentException
        /// </summary>
        /// <param name="source">Source string to test</param>
        /// <param name="name">Name of argument tested</param>
        /// <param name="error">Error message which will be formated with <paramref name="name"/> parameter</param>
        public static void CheckMandatoryOption(this string source, string name, string error)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException(string.Format(error, name));
        }

        /// <summary>
        /// Throw if argument is not of the required type
        /// </summary>
        /// <param name="source">Source object to check</param>
        /// <param name="name">Name of argument</param>
        public static void CheckArgumentOfType<T>(this object source, string name)
        {
            if (!(source is T))
                throw new ArgumentException(string.Format(Errors.Exception_WrongType, name, typeof(T).ToString()));
        }
    }
}
