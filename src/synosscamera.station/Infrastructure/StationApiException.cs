using Newtonsoft.Json;
using synosscamera.core.Extensions;
using synosscamera.core.Infrastructure.Http;
using synosscamera.core.Model.Dto;
using synosscamera.station.Model;
using System;
using System.Net;

namespace synosscamera.station.Infrastructure
{
    /// <summary>
    /// Station API Exception
    /// </summary>
    public class StationApiException : ApiClientException
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public StationApiException() : base()
        {

        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="message">Error message</param>
        public StationApiException(string message) : base(message)
        {

        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public StationApiException(string message, Exception innerException) : base(message, innerException)
        {

        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="reason">Reason causing the error</param>
        /// <param name="content">Response content</param>
        /// <param name="status">Http status code</param>
        public StationApiException(string message, string reason, string content, HttpStatusCode status) : base(message, reason, content, status)
        {
        }

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="reason">Reason causing the error</param>
        /// <param name="content">Response content</param>
        /// <param name="status">Http status code</param>
        /// <param name="innerException">Inner exception</param>
        public StationApiException(string message, string reason, string content, HttpStatusCode status, Exception innerException)
            : base(message, reason, content, status, innerException)
        {
        }
        /// <summary>
        /// Update status code
        /// </summary>
        /// <param name="statusCode"></param>
        public void UpdateStatusCode(HttpStatusCode statusCode)
        {
            ResponseStatus = statusCode;
        }
        /// <inheritdoc/>
        protected override ApiErrorResponse ApiErrorResponseFromContent(string content)
        {
            if (content.IsPresent())
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<StationResponseBase>(content);
                    if (response?.Error != null)
                    {
                        return new ApiErrorResponse()
                        {
                            IsApiError = true,
                            Errors = new ApiError[]
                            {
                                new ApiError() {
                                    ExternalErrorCode = response.Error.Code.ToString(),
                                    ErrorType = ApiErrorTypes.ExternalRestApi,
                                    ErrorCode = response.Error.Code.ToString()
                                }
                            }
                        };
                    }
                }
                catch
                {

                }
            }

            return null;
        }
    }
}
