using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.Clients
{
    public class IgnoreUnsafeHeadersHandler : DelegatingHandler
    {
        public IgnoreUnsafeHeadersHandler()
        {
            InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headerName = "Redirect Access-Control-Allow-Origin";
            var r = await base.SendAsync(request, cancellationToken);
            var header = r.Headers.FirstOrDefault(x => x.Key == headerName);
            var updatedValue = header.Value.Select(x => x.StartsWith("\"") ? x : "\"" + x + "\"").ToList();
            r.Headers.Remove(headerName);
            r.Headers.Add(headerName, updatedValue);
            return r;
        }
    }
}
