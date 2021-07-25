using synosscamera.station.Model.ApiInfo;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.station.Abstractions
{
    /// <summary>
    /// Internal interface which allows station APIs use common functions of other APis
    /// </summary>
    public interface IStationApiUtil
    {
        /// <summary>
        /// Get the list of available APIs
        /// </summary>
        /// <param name="cancellationToken">Cacnellation token.</param>
        /// <returns>A dictionary with </returns>
        Task<Dictionary<string, ApiDetails>> ApiList(CancellationToken cancellationToken = default);
        /// <summary>
        /// Login the configured user.
        /// </summary>
        /// <remarks>Will update all SynoTokens in all <see cref="IStationApi"/> implementations.</remarks>
        /// <param name="cancellationToken">Cacnellation token.</param>
        /// <returns>True if succeed.</returns>
        Task<bool> Login(CancellationToken cancellationToken = default);
        /// <summary>
        /// Logout the current user session
        /// </summary>
        /// <remarks>Will update all SynoTokens in all <see cref="IStationApi"/> implementations.</remarks>
        /// <param name="cancellationToken">Cacnellation token.</param>
        /// <returns>True if succeed.</returns>
        Task<bool> Logout(CancellationToken cancellationToken = default);
    }
}
