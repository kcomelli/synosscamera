using Newtonsoft.Json;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using synosscamera.core.Model.Dto;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure.Http
{
    /// <summary>
    /// Rest api client exception
    /// </summary>
    public class ApiClientException : Exception
    {
        /// <summary>
        /// Reason causing the error
        /// </summary>
        public string ReasonPhrase { get; private set; }
        /// <summary>
        /// Response content
        /// </summary>
        public string ResponseContent { get; private set; }
        /// <summary>
        /// response status code
        /// </summary>
        public HttpStatusCode ResponseStatus { get; protected set; }
        /// <summary>
        /// If an error occured, the API client will try to produce a meaningful
        /// ApiErrorResponse object for returning data to the clients.
        /// </summary>
        public ApiErrorResponse ErrorResponse { get; set; }

        /// <summary>
        /// Constructor of the class
        /// </summary>
        public ApiClientException() : base()
        {

        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="message">Error message</param>
        public ApiClientException(string message) : base(message)
        {

        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public ApiClientException(string message, Exception innerException) : base(message, innerException)
        {

        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="reason">Reason causing the error</param>
        /// <param name="content">Response content</param>
        /// <param name="status">Http status code</param>
        public ApiClientException(string message, string reason, string content, HttpStatusCode status) : base(message)
        {
            ReasonPhrase = reason;
            ResponseContent = content;
            ResponseStatus = status;
            ErrorResponse = ApiErrorResponseFromContent(content);
        }

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="reason">Reason causing the error</param>
        /// <param name="content">Response content</param>
        /// <param name="status">Http status code</param>
        /// <param name="innerException">Inner exception</param>
        public ApiClientException(string message, string reason, string content, HttpStatusCode status, Exception innerException) : base(message, innerException)
        {
            ReasonPhrase = reason;
            ResponseContent = content;
            ResponseStatus = status;
            ErrorResponse = ApiErrorResponseFromContent(content);
        }

        /// <summary>
        /// Generate exception from current <see cref="HttpResponseMessage"/> instance
        /// </summary>
        /// <param name="response">Response instance</param>
        /// <param name="message">Additional error message</param>
        /// <returns>An exception instance initialized with the data from the http error message</returns>
        public async static Task<ApiClientException> FromHttpResponseAsync(HttpResponseMessage response, string message = "")
        {
            response.CheckArgumentNull(nameof(HttpResponseMessage));

            var content = (string)null;

            if (response?.Content != null)
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return new ApiClientException(message, response.ReasonPhrase, content, response.StatusCode);
        }

        protected virtual ApiErrorResponse ApiErrorResponseFromContent(string content)
        {
            if (content.IsPresent())
            {
                try
                {
                    return JsonConvert.DeserializeObject<ApiErrorResponse>(content);
                }
                catch
                {

                }
            }

            return null;
        }
    }
}
