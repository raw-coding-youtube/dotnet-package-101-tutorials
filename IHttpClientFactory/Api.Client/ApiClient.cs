using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http.Json;

namespace Api.Client
{
    public class ApiClient
    {
        private readonly HttpClient client;
        private string _guidy;

        public ApiClient(HttpClient client)
        {
            this.client = client;

            // base configuration
            _guidy = Guid.NewGuid().ToString();
            client.DefaultRequestHeaders.Add("StartupHeader", Guid.NewGuid().ToString());
        }

        public Task<Home> GetHome()
        {
            return client.GetFromJsonAsync<Home>($"/homes/{_guidy}");
        }
    }
}
