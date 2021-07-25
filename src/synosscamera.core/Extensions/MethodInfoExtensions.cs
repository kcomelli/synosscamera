using System.Linq;
using System.Reflection;

namespace synosscamera.core.Extensions
{
    /// <summary>
    /// Method info extensions
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Get normalized name
        /// </summary>
        /// <param name="methodInfo">Methodinfo to check</param>
        /// <returns>Normalized method name</returns>
        /// <remarks>Method names containing '.' are considered explicitly implemented interface methods so the interface portion will be removed during normalization.</remarks>
        public static string GetNormalizedName(this MethodInfo methodInfo)
        {
            // Method names containing '.' are considered explicitly implemented interface methods
            // https://stackoverflow.com/a/17854048/1398672
            return methodInfo.Name.Contains(".") && methodInfo.IsFinal && methodInfo.IsPrivate
                ? methodInfo.Name.Split('.').Last()
                : methodInfo.Name;
        }
    }
}
