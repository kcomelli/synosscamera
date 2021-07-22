using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Extensions
{
    /// <summary>
    /// Url specific string extensions
    /// </summary>
    public static class UrlStringExtensions
    {
        /// <summary>
        /// Adds an endpoint (url-path) to an existing url string
        /// </summary>
        /// <param name="url">url string</param>
        /// <param name="endpointUri">endpoint path to append</param>
        /// <returns>returns the combined url as string</returns>
        [DebuggerStepThrough]
        public static string AppendEndpoint(this string url, string endpointUri)
        {
            if (string.IsNullOrEmpty(url))
                return endpointUri;

            if (string.IsNullOrEmpty(endpointUri))
                return url;

            return ($"{url}{endpointUri.EnsureLeadingSlash()}").Replace("//", "/").Replace(":/", "://");
        }

        /// <summary>
        /// Ensure a leading slash of the url string
        /// </summary>
        /// <param name="url">Relative url string</param>
        /// <returns>The string with an added leading slash</returns>
        [DebuggerStepThrough]
        public static string EnsureLeadingSlash(this string url)
        {
            if (!url.StartsWith("/"))
            {
                return "/" + url;
            }

            return url;
        }
        /// <summary>
        /// Ensure a trailing slash of the url string
        /// </summary>
        /// <param name="url">Relative url string</param>
        /// <returns>The string with an added trailing slash</returns>
        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }
        /// <summary>
        /// Remove a leading slash form an url string
        /// </summary>
        /// <param name="url">Relative url string</param>
        /// <returns>The string without a leading slash</returns>
        [DebuggerStepThrough]
        public static string RemoveLeadingSlash(this string url)
        {
            if (url != null && url.StartsWith("/"))
            {
                url = url.Substring(1);
            }

            return url;
        }
        /// <summary>
        /// Remove a trailing slash form an url string
        /// </summary>
        /// <param name="url">Relative url string</param>
        /// <returns>The string without a trailing slash</returns>
        [DebuggerStepThrough]
        public static string RemoveTrailingSlash(this string url)
        {
            if (url != null && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }
        /// <summary>
        /// Returns a clean url path
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string CleanUrlPath(this string url)
        {
            if (String.IsNullOrWhiteSpace(url)) url = "/";

            if (url != "/" && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        /// <summary>
        /// <para>Checks if an url is local (relative or absolut to current web)</para>
        /// <para>Urls must not contain scheme, protocoll or ports.<br/>
        /// Allows "/" or "/foo" but not "//" or "/\".<br/>
        /// Allows "~/" or "~/foo".<br/>
        /// </para>
        /// </summary>
        /// <param name="url">url to check</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsLocalUrl(this string url)
        {
            return
                !String.IsNullOrEmpty(url) &&

                // Allows "/" or "/foo" but not "//" or "/\".
                ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) ||

                // Allows "~/" or "~/foo".
                (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }
        /// <summary>
        /// Add a querystring parameter to the url
        /// </summary>
        /// <param name="url">Url to enhance</param>
        /// <param name="query">Query to add</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string AddQueryString(this string url, string query)
        {
            if (query.IsPresent())
            {
                if (!url.Contains("?"))
                {
                    url += "?";
                }
                else if (!url.EndsWith("&"))
                {
                    url += "&";
                }

                return url + query;
            }

            return url;
        }
        /// <summary>
        /// Add a hash fragment to an url string
        /// </summary>
        /// <param name="url">Url to enhance</param>
        /// <param name="query">Hashfragment to add</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string AddHashFragment(this string url, string query)
        {
            if (!url.Contains("#"))
            {
                url += "#";
            }

            return url + query;
        }
        /// <summary>
        /// Get the querystring of an url
        /// </summary>
        /// <param name="url">Url to check</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string GetQueryString(this string url)
        {
            if (url?.IndexOf("?", StringComparison.Ordinal) >= 0)
            {
                return url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);
            }
            else if (url?.IndexOf("#", StringComparison.Ordinal) >= 0)
            {
                return url.Substring(url.IndexOf("#", StringComparison.Ordinal) + 1);
            }
            //return url;
            return string.Empty; // no query string in url
        }
        /// <summary>
        /// Get the query string as name value collection
        /// </summary>
        /// <param name="url">Url including query string</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static NameValueCollection ReadQueryStringAsNameValueCollection(this string url)
        {
            if (url.IsPresent())
            {
                var idx = url.IndexOf('?');
                if (idx >= 0)
                {
                    url = url.Substring(idx + 1);
                }

                var query = QueryHelpers.ParseNullableQuery(url);
                if (query != null)
                {
                    return query.AsNameValueCollection();
                }

            }

            return new NameValueCollection();
        }

        /// <summary>
        /// Get the url origin
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string GetOrigin(this string url)
        {
            if (url != null && (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://")))
            {
                var idx = url.IndexOf("//", StringComparison.Ordinal);
                if (idx > 0)
                {
                    idx = url.IndexOf("/", idx + 2, StringComparison.Ordinal);
                    if (idx >= 0)
                    {
                        url = url.Substring(0, idx);
                    }
                    return url;
                }
            }

            return null;
        }
    }
}
