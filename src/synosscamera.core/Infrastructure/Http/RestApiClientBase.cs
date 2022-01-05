using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using synosscamera.core.Abstractions;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure.Http
{
    /// <summary>
    /// Base class for calling REST api endpoints
    /// </summary>
    public abstract class RestApiClientBase
    {
        private readonly ILogger _logger;
        private readonly IMemoryCacheWrapper _cache;
        private readonly IHttpClientFactory _clientFactory;

        private readonly Version _defaultRequestVersion = HttpVersion.Version11;

        private HttpClient _client;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="loggerFactory">Logger factory for creating loggers</param>
        /// <param name="cache">Local memory cache</param>
        protected RestApiClientBase(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, IMemoryCacheWrapper cache)
        {
            clientFactory.CheckArgumentNull(nameof(clientFactory));
            loggerFactory.CheckArgumentNull(nameof(loggerFactory));
            cache.CheckArgumentNull(nameof(cache));

            _clientFactory = clientFactory;
            _logger = CreateLogger(loggerFactory);
            _cache = cache;

            JsonDeserializerSettings = new JsonSerializerSettings();
            JsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                PreserveReferencesHandling = PreserveReferencesHandling.None
            };
        }

        /// <summary>
        /// Get the logger associtated with this client
        /// </summary>
        protected ILogger Logger => _logger;
        /// <summary>
        /// Get the local memory cache associated with this client
        /// </summary>
        protected IMemoryCacheWrapper Cache => _cache;
        /// <summary>
        /// Gets the http client factory
        /// </summary>
        protected IHttpClientFactory ClientFactory => _clientFactory;
        /// <summary>
        /// Api backend uri which must be set in an inheriting class
        /// </summary>
        protected abstract string ApiUri { get; }
        /// <summary>
        /// A custom context variable to store request specific information which may be used in HandleNonSuccessCode() method
        /// </summary>
        public object RequestContext { get; set; }
        /// <summary>
        /// Last api client error
        /// </summary>
        public Exception LastError { get; private set; }

        /// <summary>
        /// Json serializer settings to use for deserializing data from the api endpoint
        /// </summary>
        protected JsonSerializerSettings JsonDeserializerSettings { get; set; }
        /// <summary>
        /// Json serializer settings to use for serializing data for the api endpoint
        /// </summary>
        protected JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Create a querystring from a key value dictionary
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string CreateQueryString(Dictionary<string, object> parameter)
        {
            StringBuilder ret = new StringBuilder(500);

            foreach (var key in parameter.Keys)
            {
                if (ret.Length > 0)
                    ret.Append("&");

                if (parameter[key] != null && !parameter[key].GetType().IsValueType && !(parameter[key] is string))
                {
                    ret.Append($"{key}={(parameter[key] == null ? "" : WebUtility.UrlEncode(JsonConvert.SerializeObject(parameter[key])))}");
                }
                else
                {
                    ret.Append($"{key}={(parameter[key] == null ? "" : WebUtility.UrlEncode(Convert.ToString(parameter[key])))}");
                }
            }

            return ret.ToString();
        }
        /// <summary>
        /// Override this method to create a logger for the concrete class implementation
        /// </summary>
        /// <param name="loggerFactory">Loggerfactory to create a new logger instance</param>
        /// <returns>The logger instance used by this client</returns>
        protected virtual ILogger CreateLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Logs an api client error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns>True if handled, otherwise false which will rethrow the exception</returns>
        protected virtual bool LogError(string message, Exception ex)
        {
            return false;
        }

        /// <summary>
        /// Populate REST API headers to the http client prior to the call
        /// </summary>
        /// <param name="client">Http client to use</param>
        /// <param name="state">Per request state object</param>
        /// <param name="cancellation"></param>
        protected virtual Task PopulateOptionsToClientHeaders(HttpClient client, IRestApiRequestState state = null, CancellationToken cancellation = default)
        {
            ApiUri.CheckArgumentNullOrEmpty("ApiEndpoint");

            Logger.LogDebug("Using ApiUri '{apiUri}'", ApiUri.EnsureTrailingSlash());
            if(client.BaseAddress == null)
                client.BaseAddress = new Uri(ApiUri.EnsureTrailingSlash());

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (state != null)
            {
                if (state.NonMatchEtags?.Count > 0)
                    state.NonMatchEtags.ForEach(etag => client.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue(etag)));
                if (state.MatchEtags?.Count > 0)
                    state.MatchEtags.ForEach(etag => client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue(etag)));
            }

            cancellation.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Post async
        /// </summary>
        /// <typeparam name="T">Object type which should be used to deserialize JSON result data from the API</typeparam>
        /// <param name="apiAction">API action (will be appended to the endpoint uri)</param>
        /// <param name="data">Data to post to the API (will be converted to JSON)</param>
        /// <param name="uriParameters">Uri parameter to send to the API (must be uri encoded)</param>
        /// <param name="state">Per request state object</param>
        /// <param name="token"></param>
        /// <returns>A deserialized object of the specified type</returns>
        public virtual async Task<T> CallPostApiAsync<T>(string apiAction, object data, string uriParameters = "", IRestApiRequestState state = null, CancellationToken token = default(CancellationToken))
        {
            await VerifyOptions(token).ConfigureAwait(false);
            await VerifySecurityAsync(token).ConfigureAwait(false);

            //using (var client = await GetHttpClient().ConfigureAwait(false))
            var client = await GetHttpClient();
            {
                token.ThrowIfCancellationRequested();
                var targetRequestUri = apiAction.RemoveLeadingSlash() + (string.IsNullOrEmpty(uriParameters) ? "" : "?" + uriParameters);

                await PopulateOptionsToClientHeaders(client, state, token).ConfigureAwait(false);
                var dtStart = DateTime.UtcNow;
                RequestSending(HttpMethod.Post, client, apiAction, ToJson(data));
                var content = GetContent(data);

                token.ThrowIfCancellationRequested();

                var request = CreateRequestMessage(HttpMethod.Post, CreateUri(targetRequestUri));
                request.Content = content;

                var response = await client.SendAsync(request, token).ConfigureAwait(false);

                var responseString = string.Empty;
                if (response?.Content != null)
                    responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                UpdateStateWithResponseAndRequest(state, response, request);

                ResponseStringReceived(response.StatusCode, HttpMethod.Post, responseString, (DateTime.UtcNow - dtStart).TotalMilliseconds);

                if (response.IsSuccessStatusCode)
                {
                    if (responseString.IsPresent())
                    {
                        var apiResponse = JsonConvert.DeserializeObject<T>(responseString, JsonDeserializerSettings);
                        if (apiResponse is IEtagResponse)
                            SetEtag(response, apiResponse as IEtagResponse);

                        return apiResponse;
                    }

                    return default(T);
                }
                else
                {
                    return await InternalHandleNonSuccessCodeAsync<T>(response, (DateTime.UtcNow - dtStart).TotalMilliseconds); ;
                }
            }
        }
        /// <summary>
        /// Put async
        /// </summary>
        /// <typeparam name="T">Object type which should be used to deserialize JSON result data from the API</typeparam>
        /// <param name="apiAction">API action (will be appended to the endpoint uri)</param>
        /// <param name="data">Data to put to the API (will be converted to JSON)</param>
        /// <param name="uriParameters">Uri parameter to send to the API (must be uri encoded)</param>
        /// <param name="state">Per request state object</param>
        /// <param name="token"></param>
        /// <returns>A deserialized object of the specified type</returns>
        public virtual async Task<T> CallPutApiAsync<T>(string apiAction, object data, string uriParameters = "", IRestApiRequestState state = null, CancellationToken token = default(CancellationToken))
        {
            await VerifyOptions(token).ConfigureAwait(false);
            await VerifySecurityAsync(token).ConfigureAwait(false);

            //using (var client = await GetHttpClient().ConfigureAwait(false))
            var client = await GetHttpClient();
            {
                var targetRequestUri = apiAction.RemoveLeadingSlash() + (string.IsNullOrEmpty(uriParameters) ? "" : "?" + uriParameters);

                await PopulateOptionsToClientHeaders(client, state, token).ConfigureAwait(false);
                var dtStart = DateTime.UtcNow;
                RequestSending(HttpMethod.Put, client, apiAction, ToJson(data));
                var content = GetContent(data);

                var request = CreateRequestMessage(HttpMethod.Put, CreateUri(targetRequestUri));
                request.Content = content;

                var response = await client.SendAsync(request, token).ConfigureAwait(false);

                var responseString = string.Empty;
                if (response?.Content != null)
                    responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                UpdateStateWithResponseAndRequest(state, response, request);

                ResponseStringReceived(response.StatusCode, HttpMethod.Put, responseString, (DateTime.UtcNow - dtStart).TotalMilliseconds);

                if (response.IsSuccessStatusCode)
                {
                    if (responseString.IsPresent())
                    {
                        var apiResponse = JsonConvert.DeserializeObject<T>(responseString, JsonDeserializerSettings);
                        if (apiResponse is IEtagResponse)
                            SetEtag(response, apiResponse as IEtagResponse);

                        return apiResponse;
                    }

                    return default(T);
                }
                else
                {
                    return await InternalHandleNonSuccessCodeAsync<T>(response, (DateTime.UtcNow - dtStart).TotalMilliseconds); ;
                }
            }
        }
        /// <summary>
        /// Delete async
        /// </summary>
        /// <typeparam name="T">Object type which should be used to deserialize JSON result data from the API</typeparam>
        /// <param name="apiAction">API action (will be appended to the endpoint uri)</param>
        /// <param name="uriParameters">Uri parameter to send to the API (must be uri encoded)</param>
        /// <param name="state">Per request state object</param>
        /// <param name="token"></param>
        /// <returns>A deserialized object of the specified type</returns>
        public virtual async Task<T> CallDeleteApiAsync<T>(string apiAction, string uriParameters = "", IRestApiRequestState state = null, CancellationToken token = default(CancellationToken))
        {
            await VerifyOptions(token).ConfigureAwait(false);
            await VerifySecurityAsync(token).ConfigureAwait(false);

            //using (var client = await GetHttpClient().ConfigureAwait(false))
            var client = await GetHttpClient();
            {
                var targetRequestUri = apiAction.RemoveLeadingSlash() + (string.IsNullOrEmpty(uriParameters) ? "" : "?" + uriParameters);
                await PopulateOptionsToClientHeaders(client, state, token).ConfigureAwait(false);

                var dtStart = DateTime.UtcNow;
                RequestSending(HttpMethod.Delete, client, apiAction.RemoveLeadingSlash(), (string.IsNullOrEmpty(uriParameters) ? "" : "?" + uriParameters));

                var request = CreateRequestMessage(HttpMethod.Delete, CreateUri(targetRequestUri));

                var response = await client.SendAsync(request, token).ConfigureAwait(false);

                var responseString = string.Empty;
                if (response?.Content != null)
                    responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                UpdateStateWithResponseAndRequest(state, response, request);

                ResponseStringReceived(response.StatusCode, HttpMethod.Delete, responseString, (DateTime.UtcNow - dtStart).TotalMilliseconds);

                if (response.IsSuccessStatusCode)
                {
                    if (responseString.IsPresent())
                    {
                        var apiResponse = JsonConvert.DeserializeObject<T>(responseString, JsonDeserializerSettings);
                        if (apiResponse is IEtagResponse)
                            SetEtag(response, apiResponse as IEtagResponse);

                        return apiResponse;
                    }

                    return default(T);
                }
                else
                {
                    return await InternalHandleNonSuccessCodeAsync<T>(response, (DateTime.UtcNow - dtStart).TotalMilliseconds); ;
                }
            }
        }

        /// <summary>
        /// Get async
        /// </summary>
        /// <typeparam name="T">Object type which should be used to deserialize JSON result data from the API</typeparam>
        /// <param name="apiAction">API action (will be appended to the endpoint uri)</param>
        /// <param name="uriParameters">Uri parameter to send to the API (must be uri encoded)</param>
        /// <param name="state">Per request state object</param>
        /// <param name="token"></param>
        /// <returns>A deserialized object of the specified type</returns>
        public virtual async Task<T> CallGetApiAsync<T>(string apiAction, string uriParameters = "", IRestApiRequestState state = null, CancellationToken token = default(CancellationToken))
        {
            await VerifyOptions(token).ConfigureAwait(false);
            await VerifySecurityAsync(token).ConfigureAwait(false);

            //using (var client = await GetHttpClient().ConfigureAwait(false))
            var client = await GetHttpClient();
            {
                var targetRequestUri = apiAction.RemoveLeadingSlash() + (string.IsNullOrEmpty(uriParameters) ? "" : "?" + uriParameters);
                await PopulateOptionsToClientHeaders(client, state, token).ConfigureAwait(false);

                var dtStart = DateTime.UtcNow;

                RequestSending(HttpMethod.Get, client, apiAction.RemoveLeadingSlash(), (string.IsNullOrEmpty(uriParameters) ? "" : "?" + uriParameters));

                var request = CreateRequestMessage(HttpMethod.Get, CreateUri(targetRequestUri));

                var response = await client.SendAsync(request, token).ConfigureAwait(false);

                var responseString = string.Empty;
                if (response?.Content != null)
                    responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                UpdateStateWithResponseAndRequest(state, response, request);

                ResponseStringReceived(response.StatusCode, HttpMethod.Get, responseString, (DateTime.UtcNow - dtStart).TotalMilliseconds);

                if (response.IsSuccessStatusCode)
                {
                    if (responseString.IsPresent())
                    {
                        var apiResponse = JsonConvert.DeserializeObject<T>(responseString, JsonDeserializerSettings);
                        if (apiResponse is IEtagResponse)
                            SetEtag(response, apiResponse as IEtagResponse);

                        return apiResponse;
                    }

                    return default(T);
                }
                else
                {
                    return await InternalHandleNonSuccessCodeAsync<T>(response, (DateTime.UtcNow - dtStart).TotalMilliseconds); ;
                }
            }
        }

        private void UpdateStateWithResponseAndRequest(IRestApiRequestState state, HttpResponseMessage response, HttpRequestMessage request)
        {
            if (state != null)
            {
                state.StatusCode = response.StatusCode;
                state.ResponseHeaders = response.Headers;
                state.RequestProperties = request?.Properties?.ToDictionary(k => k.Key, v => v.Value);

                OnUpdateStateWithResponseAndRequest(state, response, request);
            }
        }

        /// <summary>
        /// Only called if state is not null!
        /// </summary>
        /// <param name="state">State which was updated</param>
        /// <param name="response">Response fetched</param>
        /// <param name="request">Request generated and used</param>
        protected virtual void OnUpdateStateWithResponseAndRequest(IRestApiRequestState state, HttpResponseMessage response, HttpRequestMessage request)
        {

        }

        private void SetEtag(HttpResponseMessage response, IEtagResponse data)
        {
            if (response?.Headers?.ETag == null || data == null)
                return;

            var etagValue = response.Headers.ETag;
            data.Etag = etagValue.Tag;
        }

        /// <summary>
        /// Handle non success code - may throw an exception if not handled in an inherited class implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="millisecondsNeeded">Milliseconds needed for the request</param>
        /// <returns></returns>
        private async Task<T> InternalHandleNonSuccessCodeAsync<T>(HttpResponseMessage response, double millisecondsNeeded)
        {
            var responseString = "";

            if (response?.Content != null)
                responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // remove status code from response
            if (responseString?.IndexOf(":") == 3)
                responseString = responseString.Substring(4).Trim();

            Logger.LogDebug("Recevied API-Client non-success response: {statusCode} - '{response}'", response?.StatusCode ?? 0, responseString ?? "null");

            LastError = await ExceptionFromResponse(response);

            var returnValue = await HandleNonSuccessCode<T>(response, millisecondsNeeded).ConfigureAwait(false);

            if (!returnValue.handled)
            {
                Logger.LogError(LastError, "Recevied API-Client error response: {statusCode} - '{response}'", response?.StatusCode ?? 0, responseString ?? "null");
                response.CheckArgumentNull(nameof(response));
                throw LastError;
            }

            return returnValue.returnValue;
        }
        /// <summary>
        /// Get api exception from response
        /// </summary>
        /// <param name="response"></param>
        /// <param name="canellation"></param>
        /// <returns></returns>
        protected virtual async Task<ApiClientException> ExceptionFromResponse(HttpResponseMessage response, CancellationToken canellation = default)
        {
            if (response != null)
                return await ApiClientException.FromHttpResponseAsync(response, "REST Api error - " + (response?.StatusCode.ToString() ?? "0") + " (" + response.ReasonPhrase + ")");

            return null;
        }
        /// <summary>
        /// Allows a client implementation to handle a non-success status code. 
        /// If handled, the client should return true in order to avoid exceptions 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="millisecondsNeeded">Milliseconds needed for the request</param>
        /// <returns>True if handled by the client implementation, otherwise false</returns>
        protected virtual Task<(T returnValue, bool handled)> HandleNonSuccessCode<T>(HttpResponseMessage response, double millisecondsNeeded)
        {
            return Task.FromResult((default(T), false));
        }

        /// <summary>
        /// For internal logging purpose. Called with the raw response string read from the http response
        /// </summary>
        /// <param name="method">Method executed</param>
        /// <param name="response">Response string</param>
        /// <param name="statusCode">Status code</param>
        /// <param name="millisecondsNeeded">Milliseconds needed for the request</param>
        protected virtual void ResponseStringReceived(HttpStatusCode statusCode, HttpMethod method, string response, double millisecondsNeeded)
        {

        }
        /// <summary>
        /// For internal logging purpose. Called with the data sent to the endpoint
        /// </summary>
        /// <param name="method">Method executed</param>
        /// <param name="client">HttpClient used (extract default headers) and Base URI</param>
        /// <param name="action">Action used</param>
        /// <param name="data">Body or querystring data</param>
        protected virtual void RequestSending(HttpMethod method, HttpClient client, string action, string data)
        {

        }

        /// <summary>
        /// Get <see cref="HttpContent"/> based on data depending on the client type (e.g. json, formdata, etc).
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract HttpContent GetContent(object data);

        /// <summary>
        /// Convert data object to json string
        /// </summary>
        /// <param name="data"></param>
        protected string ToJson(object data)
        {
            if (data == null)
                return string.Empty;

            return JsonConvert.SerializeObject(data, data.GetType(), JsonSerializerSettings);
        }

        /// <summary>
        /// Verify if all options are set for a call to the api
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual Task VerifyOptions(CancellationToken token = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns an http client instance. May be overwritten (e.g. UnitTesting)
        /// </summary>
        /// <returns></returns>
        protected virtual Task<HttpClient> GetHttpClient()
        {
            // we use named clients
            // just register your client configuration by using the class/type name of the api client
            // and you will get the corrent instance here

            if(_client == null)
                _client = _clientFactory.CreateClient(this.GetType().Name);

            return Task.FromResult(_client);
        }

        /// <summary>
        /// Create request message
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri uri) =>
            new HttpRequestMessage(method, uri) { Version = _defaultRequestVersion };

        /// <summary>
        /// Create uri from string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected Uri CreateUri(string uri) =>
            string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);

        /// <summary>
        /// Implements security related checks and code e.g. retrieving bearer token
        /// </summary>
        /// <param name="token"></param>
        protected virtual Task VerifySecurityAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }
}
