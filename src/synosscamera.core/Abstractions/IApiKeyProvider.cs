using synosscamera.core.Configuration;
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
        /// <returns>Returns a tuple continging info if the key is valid and if yes, its data</returns>
        Task<(bool valid, ApiKeyData keyData)> VerifyApiKey(string apiKey, CancellationToken cancellationToken = default);
    }
}
