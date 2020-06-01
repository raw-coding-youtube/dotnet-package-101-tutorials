using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Client
{
    public class HttpContextMiddleware : DelegatingHandler
    {
        private string _ctor;

        public HttpContextMiddleware()
        {
            _ctor = Guid.NewGuid().ToString();
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var method = Guid.NewGuid().ToString();

            request.Headers.Add("Middleware-Ctor", _ctor);
            request.Headers.Add("Middleware-Method", method);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
