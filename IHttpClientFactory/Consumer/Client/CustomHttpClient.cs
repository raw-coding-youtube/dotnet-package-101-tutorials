using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Client
{
    public class CustomHttpClient
    {
        private readonly HttpClient client;
        private string _guidy;

        public CustomHttpClient(HttpClient client)
        {
            this.client = client;

            // base configuration
            _guidy = Guid.NewGuid().ToString();
            client.DefaultRequestHeaders.Add("StartupHeader", Guid.NewGuid().ToString());
        }

        public Task<string> GetHome()
        {
            return client.GetStringAsync($"/homes/{_guidy}");
        }
    }
}
