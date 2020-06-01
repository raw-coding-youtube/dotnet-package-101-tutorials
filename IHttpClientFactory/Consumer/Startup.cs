using Api.Client;
using Consumer.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Consumer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddHttpClient("simple", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5003");

                client.DefaultRequestHeaders.Add("StartupHeader", Guid.NewGuid().ToString());
            });

            //services.AddHttpClient<CustomHttpClient>("custom", client =>
            //{
            //    client.BaseAddress = new Uri("https://localhost:5003");
            //})
            //    .AddHttpMessageHandler<HttpContextMiddleware>();

            services.AddApiClient(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5003");
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
