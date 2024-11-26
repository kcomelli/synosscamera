using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.core.Extensions
{
    /// <summary>
    /// Exception extensions
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Checks if the given exception or any inner exception is of the provided type
        /// </summary>
        /// <typeparam name="TException">Type of the exception to look for.</typeparam>
        /// <param name="ex">Exception which should be inspected.</param>
        /// <returns>True if the exception of the given type was found.</returns>
        public static bool ContainsExceptionOfType<TException>(this Exception ex)
        {
            if (ex != null)
            {
                if (ex is TException) return true;

                if (ex.InnerException != null)
                    return ContainsExceptionOfType<TException>(ex.InnerException);
            }

            return false;
        }
    }
}
