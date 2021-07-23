using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using synosscamera.core;
using synosscamera.core.Abstractions;
using synosscamera.core.Diagnostics;
using synosscamera.core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace synosscamera.api.Infrastructure.Authentication
{
    /// <summary>
    /// API Key authentication handler
    /// </summary>
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IApiKeyProvider _apiKeyProvider;
        private readonly ILogger _logger;
        private readonly IDistributedCacheWrapper _cacheWrapper;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="apiKeyProvider"></param>
        /// <param name="cacheWrapper"></param>
        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IDistributedCacheWrapper cacheWrapper, IApiKeyProvider apiKeyProvider)
            : base(options, logger, encoder, clock)
        {
            logger.CheckArgumentNull(nameof(logger));
            apiKeyProvider.CheckArgumentNull(nameof(apiKeyProvider));
            cacheWrapper.CheckArgumentNull(nameof(cacheWrapper));

            _cacheWrapper = cacheWrapper;
            _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
            _apiKeyProvider = apiKeyProvider;
        }

        private string TokenFromPathOrQueryString()
        {
            _logger.LogDebug("Try to extract api key from request path.");

            if (Options.ReadKeyFromPath)
            {
                var lastValue = Request.Path.ToString().RemoveTrailingSlash().Split('/').LastOrDefault();
                if (lastValue.IsPresent())
                {
                    _logger.LogInformation("Extracted valid api key token from request path.");
                    _logger.LogTrace("Extracted api key {apiKey} from request path for scheme '{scheme}'.", lastValue, Scheme.Name);
                    return lastValue;
                }

                _logger.LogDebug("No valid api key token found in request path.");
            }
            else
            {
                _logger.LogDebug("Skipped api key from path extraction because it is not enabled for scheme '{scheme}'.", this.Scheme.Name);
            }

            if (Options.AllowTokenInQueryString)
            {
                if (Request.Query.TryGetValue(Options.TokenQueryParameterName, out StringValues tokenFromQueryString))
                {
                    _logger.LogInformation("Extracted valid api key token from request query string.");
                    _logger.LogTrace("Extracted api key {apiKey} from request querystring for scheme '{scheme}'.", tokenFromQueryString.ToString(), Scheme.Name);
                    return tokenFromQueryString.ToString();
                }

                _logger.LogDebug("No valid api key token found in request querystring.");
            }
            else
            {
                _logger.LogDebug("Skipped api key from querystring extraction because it is not enabled for scheme '{scheme}'.", this.Scheme.Name);
            }

            return null;
        }

        /// <summary>
        /// This method is called when the authentication middleware calls the DefaultAuthenticateScheme
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                //_appRepository.QueryAsNoTracking = true;
                //_apiKeyRepository.QueryAsNoTracking = true;

                _logger.LogDebug("Begin handle authenticate for scheme '{scheme}'.", Scheme.Name);

                var tokenValue = TokenFromPathOrQueryString();

                if (!CheckIfApiKeyTokenCouldBeFound(tokenValue))
                    return AuthenticateResult.NoResult();

                if (!ExtractApiKeyToken(tokenValue, out string apiKey))
                    return AuthenticateResult.NoResult();

                if (apiKey.IsMissing())
                    return AuthenticateResult.Fail("Invalid api key authentication header");

                _logger.LogTrace("Try to authenticate api key '{apiKey}' using scheme '{scheme}'.", apiKey, Scheme.Name);

                var apiKeyValid = await _apiKeyProvider.VerifyApiKey(apiKey, Context.RequestAborted);

                if (!apiKeyValid.valid)
                    return AuthenticateResult.Fail($"Invalid api key for scheme {this.Scheme.Name}");


                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, apiKeyValid.keyData.Name ?? Constants.Security.ApiUserName),
                    new Claim(ClaimTypes.Name, apiKeyValid.keyData.Name ?? Constants.Security.ApiUserName),
                    new Claim(Constants.Security.Claims.Subject, apiKeyValid.keyData.Name ?? Constants.Security.ApiUserName),
                };

                return AuthenticateResult.Success(CreateAuthTicket(apiKey, claims));
            }
            finally
            {

            }
        }


        private bool CheckIfApiKeyTokenCouldBeFound(string extractedTokenFromPath)
        {
            // if Authorization header is missing
            if (!Request.Headers.ContainsKey(Constants.DefaultHeaders.Authorization))
            {
                // check if custom header is allowed
                if (Options.AllowCustomHeader)
                {
                    // if custom header is missing
                    if (!Request.Headers.ContainsKey(Options.CustomHeaderName))
                    {
                        // check if token from path was read
                        if (extractedTokenFromPath.IsMissing())
                            //Token from request path not valid header not in request
                            return false;
                    }
                }
                else
                {
                    // check if token from path was read
                    if (extractedTokenFromPath.IsMissing())
                        //Token from request path not valid header not in request
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Extract an api key token from the authorization header, the custom header or uses the value from the path (if configured)
        /// </summary>
        /// <param name="extractedTokenFromPath"></param>
        /// <param name="apiKey"></param>
        /// <returns>False if an existing authorization header is invalid or for a different scheme. In this case, the auth handler should return NoResult</returns>
        private bool ExtractApiKeyToken(string extractedTokenFromPath, out string apiKey)
        {
            apiKey = string.Empty;

            if (Request.Headers.ContainsKey(Constants.DefaultHeaders.Authorization))
            {
                if (!AuthenticationHeaderValue.TryParse(Request.Headers[Constants.DefaultHeaders.Authorization], out AuthenticationHeaderValue headerValue))
                    //Invalid Authorization header
                    return false;

                if (!this.Scheme.Name.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                    //Scheme does not match the configured options authentication header
                    return false;

                apiKey = headerValue.Parameter;
            }
            else
            {

                if (Options.CustomHeaderName.IsPresent() && Request.Headers.TryGetValue(Options.CustomHeaderName, out StringValues apiKeyValue))
                    apiKey = apiKeyValue.FirstOrDefault();

                // if api key could not be loaded from the custom header
                // set the api key to the loaded value from the request path (if successfull)
                if (apiKey.IsMissing())
                    apiKey = extractedTokenFromPath;
            }

            return true;
        }

        private AuthenticationTicket CreateAuthTicket(string apiKey, Claim[] claims)
        {
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            // save the auth token properties
            var authTokens = new List<AuthenticationToken>
            {
                new AuthenticationToken
                {
                    Name = Constants.Security.ApiTokenName,
                    Value = apiKey
                }
            };

            var properties = new AuthenticationProperties();
            properties.StoreTokens(authTokens);

            return new AuthenticationTicket(principal, properties, Scheme.Name);
        }

        /// <summary>
        /// Handles a 401 challenge response
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["SYNOSS-Authenticate"] = $"{this.Scheme.Name} realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
