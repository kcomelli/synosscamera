using synosscamera.station.Model.ApiInfo;
using System.Collections.Generic;

namespace synosscamera.station.Abstractions
{
    /// <summary>
    /// Common station API
    /// </summary>
    public interface IStationApi
    {
        /// <summary>
        /// Get/set the syno token
        /// </summary>
        public string SynoToken { get; set; }
        /// <summary>
        /// Sets the API list if retreived
        /// </summary>
        /// <param name="apiList">Api list to set</param>
        void SetApiList(Dictionary<string, ApiDetails> apiList);
    }
}
