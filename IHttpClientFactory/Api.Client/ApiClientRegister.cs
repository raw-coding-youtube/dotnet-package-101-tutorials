using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Api.Client
{
    public static class ApiClientRegister
    {
        public static IServiceCollection AddApiClient(
            this IServiceCollection services,
            Action<HttpClient> clientConfiguration)
        {
            services.AddTransient<HttpContextMiddleware>();

            services.AddHttpClient<ApiClient>(clientConfiguration)
                .AddHttpMessageHandler<HttpContextMiddleware>();

            return services;
        }
    }
}
