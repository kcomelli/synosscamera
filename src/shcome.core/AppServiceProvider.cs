using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shcome.core
{
    /// <summary>
    /// Application scoped service provider
    /// </summary>
    public static class AppServiceProvider
    {
        /// <summary>
        /// Get or set service provider instance
        /// </summary>
        /// <remarks>Typically a app-scoped instance.</remarks>
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Flag indicating if an OutOfMemory exception has been catched somewhere in code.
        /// In this case the health check will report faulted state to force a pod restart.
        /// </summary>
        public static bool OutOfMemoryCatched => _OutOfMemoryCatched;
        /// <summary>
        /// Additional information 
        /// </summary>
        public static string OutOfMemoryInfo => _OutOfMemoryInfo;
        /// <summary>
        /// Flag indicating if this is an unrecoverable error (e.g. background workers have 
        /// been shut down and will not restart automatically.)
        /// </summary>
        public static bool OutOfMemoryFatal => _OutOfMemoryFatal;

        /// <summary>
        /// Flag indicating if an OutOfMemory exception has been catched somewhere in code.
        /// In this case the health check will report faulted state to force a pod restart.
        /// </summary>
        private static bool _OutOfMemoryCatched { get; set; }
        /// <summary>
        /// Additional information 
        /// </summary>
        private static string _OutOfMemoryInfo { get; set; }
        /// <summary>
        /// Flag indicating if this is an unrecoverable error (e.g. background workers have 
        /// been shut down and will not restart automatically.)
        /// </summary>
        private static bool _OutOfMemoryFatal { get; set; }
        /// <summary>
        /// Set the application to OutOfMemory detected state.
        /// </summary>
        /// <param name="additionalInfo">Additional info</param>
        /// <param name="fatal">True if the error is fatal and requires pod restart!</param>
        public static void SetOutOfMemoryDetected(string additionalInfo, bool fatal = false)
        {
            _OutOfMemoryCatched = true;
            _OutOfMemoryInfo = additionalInfo;
            _OutOfMemoryFatal = _OutOfMemoryFatal | fatal;
        }
        /// <summary>
        /// Reset OutOfMemory state if it is currently not a fatal state
        /// </summary>
        public static void ResetOutOfMemoryDetected()
        {
            // fatal error cannot be reset !!
            if (_OutOfMemoryFatal) return;

            _OutOfMemoryCatched = false;
            _OutOfMemoryInfo = string.Empty;
        }
    }
}
