using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.core.Abstractions
{
    /// <summary>
    /// Interface defining a roken provider
    /// </summary>
    public interface IApiKeyProvider
    {
        /// <summary>
        /// Verify if a given api key 
        /// </summary>
        /// <param name="apiKey">Api key to check</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if the token is valid</returns>
        Task<bool> VerifyApiKey(string apiKey, CancellationToken cancellationToken = default);
    }
}
