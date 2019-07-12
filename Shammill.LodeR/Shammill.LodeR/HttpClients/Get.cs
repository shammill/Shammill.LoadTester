using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Shammill.LodeR.HttpClients
{
    public class HttpClient
    {
        public Stopwatch Stopwatch = new Stopwatch();
        public double ResponsesTime = 0d;
        public int Status = 0;

        public async Task<string> GetAsync(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            var response = ExecuteRequestAsync(client, request);

            return response.Result.Content;
        }

        private async Task<IRestResponse> ExecuteRequestAsync(RestClient client, RestRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();

            Stopwatch.Start();
            client.ExecuteAsync(request, response =>
            {
                Stopwatch.Stop();
                Console.WriteLine($"Response Status: {response.StatusCode} in {Stopwatch.Elapsed.TotalMilliseconds}ms");
                ResponsesTime = Stopwatch.Elapsed.TotalMilliseconds;
                Status = Convert.ToInt32(response.StatusCode);
                Stopwatch.Reset();
                taskCompletionSource.SetResult(response);
            });

            return await taskCompletionSource.Task;
        }
    }
}
