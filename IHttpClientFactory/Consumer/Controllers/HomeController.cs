using Api.Client;
using Consumer.Client;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Consumer.Controllers
{
    public class HomeController : Controller
    {
        public Task<string> Bad()
        {
            var client = new HttpClient { BaseAddress = new Uri("https://localhost:5003") };

            return client.GetStringAsync($"/homes/{Guid.NewGuid()}");
        }

        public Task<string> Simple([FromServices] IHttpClientFactory factory)
        {
            var client = factory.CreateClient("simple");

            return client.GetStringAsync($"/homes/{Guid.NewGuid()}");
        }


        public Task<string> Typed([FromServices] CustomHttpClient client)
        {
            return client.GetHome();
        }


        public Task<Home> Shared([FromServices] ApiClient client)
        {
            return client.GetHome();
        }
    }
}
