using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Shammill.LoadTester.HttpClients
{
    public class HttpClient
    {
        public Stopwatch Stopwatch = new Stopwatch();
        public double ResponsesTime = 0d;
        public int Status = 0;

        public async Task<string> ExecuteRequest(string url, string cookieName, string cookieValue, Method requestMethod = Method.GET, string payload = "")
        {
            var client = new RestClient(url);
            if (!String.IsNullOrEmpty(cookieName) && !String.IsNullOrEmpty(cookieValue))
            {
                client.CookieContainer = new CookieContainer();
                client.CookieContainer.Add(new Uri(url), new Cookie { Name = cookieName, Value = cookieValue, });
            }
            var request = new RestRequest(requestMethod);
            if (!String.IsNullOrEmpty(payload))
                request.AddJsonBody(payload);
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
