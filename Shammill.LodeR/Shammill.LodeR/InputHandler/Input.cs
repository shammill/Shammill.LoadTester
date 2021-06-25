using Shammill.LodeR.HttpClients;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using RestSharp;

namespace Shammill.LodeR.InputHandler
{
    public static class Input
    {
        public static void Process()
        {
            List<double> responsesTimes = new List<double>();
            List<int> statuses = new List<int>();
            string userInput = Console.ReadLine();
            string url = userInput.Split(" ")[0];
            string parameter = userInput.GetParameter();
            int numberOfRequests = Convert.ToInt32(parameter);

            string requestMethodInput = userInput.GetParameter(2);
            Method requestMethod = (Method)Enum.Parse(typeof(Method), requestMethodInput);
            string cookieName = userInput.GetParameter(3);
            string cookieValue = userInput.GetParameter(4);
            string payload = GetJsonPayload();

            Console.WriteLine("Wakeup Starting");
            // This is to 'wake up' the server - sometimes the first request/s take a lot longer.
            Parallel.For(0, 10, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                          async index =>
                          {
                              var client = new HttpClient();
                              var result = await client.ExecuteRequest(url, cookieName, cookieValue, requestMethod, payload);

                              statuses.Add(client.Status);
                              responsesTimes.Add(client.ResponsesTime);
                          });
            Console.WriteLine("Wakeup Complete");
            GetStatistics(responsesTimes, statuses);
            responsesTimes.Clear();
            statuses.Clear();

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine($"Getting {url} {parameter} times.");
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000; // probs doesnt do anything
            Parallel.For(0, numberOfRequests, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                          async index =>
                          {
                              var client = new HttpClient();
                              var result = await client.ExecuteRequest(url, cookieName, cookieValue, requestMethod, payload);

                              statuses.Add(client.Status);
                              responsesTimes.Add(client.ResponsesTime);
                          });
            Console.WriteLine("Run complete.");

            GetStatistics(responsesTimes, statuses);

        }

        private static string GetCommand(this string input)
        {
            var command = input.Split(" ");
                return command[0];
        }

        private static string GetParameter(this string input, int index = 1)
        {
            var command = input.Split(" ");

            if (command.Length >= 2)
                return command[index];

            else return "";
        }

        private static void GetStatistics(List<double> responseTimes, List<int> statuses)
        {
            responseTimes = responseTimes.OrderBy(x => x).ToList();
            var min = responseTimes.Min();
            var max = responseTimes.Max();
            
            var median = responseTimes.ElementAt(responseTimes.Count / 2);
            var average = responseTimes.Average();

            var sortedStatuses = new Dictionary<int, int>();
            var distinctStatus = statuses.Distinct();
            foreach (var status in distinctStatus)
            {
                sortedStatuses.Add(status, statuses.Count(x => x == status));
            }
            sortedStatuses.OrderBy(x => x.Key);

            Console.WriteLine("");
            Console.WriteLine("Http Status Codes:");
            foreach (var status in sortedStatuses)
            {
                Console.WriteLine($"{status.Key} - {status.Value} responses");
            }

            Console.WriteLine($"Min:{min}ms. Max:{max}ms. Median:{median}ms. Average:{average}ms.");

        }


        private static string GetJsonPayload()
        {
            return FileToMemory("payload.json");
        }

        public static string FileToMemory(string fileName)
        {
            string file = "";

            if (File.Exists(fileName))
                using (StreamReader sr = new StreamReader(fileName))
                {
                    file = sr.ReadToEnd();
                }

            return file;
        }
    }
}
