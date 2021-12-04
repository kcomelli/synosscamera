using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.core.Extensions
{
    /// <summary>
    /// Object extensions
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Convert obejct to form data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FormUrlEncodedContent ToFormData(this object obj)
        {
            var formData = (IDictionary<string, string>)null;
                        
            if (obj is IDictionary<string, string>)
                formData = obj as IDictionary<string, string>;
            else
                formData = obj.ToKeyValue();

            return new FormUrlEncodedContent(formData);
        }

        /// <summary>
        /// Convert object to disctionary
        /// </summary>
        /// <param name="metaToken"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToKeyValue(this object metaToken)
        {
            if (metaToken == null)
            {
                return new Dictionary<string,string>();
            }

            // Added by me: avoid cyclic references
            var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var token = metaToken as JToken;
            if (token == null)
            {
                // Modified by me: use serializer defined above
                return ToKeyValue(JObject.FromObject(metaToken, serializer));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                if (token.Type == JTokenType.Array)
                {
                    return new Dictionary<string, string> { { token.Path, token.ToString() } };
                }
                else
                {
                    foreach (var child in token.Children().ToList())
                    {
                        var childContent = child.ToKeyValue();
                        if (childContent != null)
                        {
                            contentData = contentData.Concat(childContent)
                                                     .ToDictionary(k => k.Key, v => v.Value);
                        }
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return new Dictionary<string, string>();
            }

            var value = jValue?.Type == JTokenType.Date ?
                            jValue?.ToString("o", CultureInfo.InvariantCulture) :
                            jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }
    }
}
