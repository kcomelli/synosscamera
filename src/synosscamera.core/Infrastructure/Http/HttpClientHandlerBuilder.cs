using System.Net;
using System.Net.Http;

namespace synosscamera.core.Infrastructure.Http
{
    /// <summary>
    /// Client http handler builder
    /// </summary>
    public class HttpClientHandlerBuilder
    {
        private readonly HttpClientHandler _handler;

        private HttpClientHandlerBuilder()
        {
            _handler = new HttpClientHandler();
        }

        /// <summary>
        /// Create a new builder
        /// </summary>
        /// <returns></returns>
        public static HttpClientHandlerBuilder Create()
        {
            return new HttpClientHandlerBuilder();
        }
        /// <summary>
        /// Set allow redirect flag
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpClientHandlerBuilder SetAllowAutoRedirect(bool value)
        {
            _handler.AllowAutoRedirect = value;
            return this;
        }
        /// <summary>
        /// Set use default credentials value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpClientHandlerBuilder SetUseDefaultCredentials(bool value)
        {
            _handler.UseDefaultCredentials = value;
            return this;
        }
        /// <summary>
        /// Set use proxy value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpClientHandlerBuilder SetUseProxy(bool value)
        {
            _handler.UseProxy = value;
            return this;
        }
        /// <summary>
        /// Set use cookies value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpClientHandlerBuilder SetUseCookies(bool value)
        {
            _handler.UseCookies = value;
            return this;
        }
        /// <summary>
        /// Set credentials to use
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpClientHandlerBuilder SetCredentials(ICredentials value)
        {
            _handler.Credentials = value;
            return this;
        }
        /// <summary>
        /// Set cookie container
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpClientHandlerBuilder SetCookieContainer(CookieContainer value)
        {
            _handler.CookieContainer = value;
            return this;
        }
        /// <summary>
        /// Set proxy to use
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpClientHandlerBuilder SetProxy(IWebProxy value)
        {
            if (_handler.SupportsProxy)
                _handler.Proxy = value;
            return this;
        }
        /// <summary>
        /// Set auto decompression methods
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpClientHandlerBuilder SetAutoDecompression(DecompressionMethods value)
        {
            if (_handler.SupportsAutomaticDecompression)
                _handler.AutomaticDecompression = value;
            return this;
        }
        /// <summary>
        /// Set accept all certificates
        /// </summary>
        /// <returns></returns>

        public HttpClientHandlerBuilder SetAcceptAllCertificates()
        {
            _handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            return this;
        }
        /// <summary>
        /// Build handler
        /// </summary>
        /// <returns></returns>
        public HttpClientHandler Build()
        {
            return _handler;
        }
    }
}
