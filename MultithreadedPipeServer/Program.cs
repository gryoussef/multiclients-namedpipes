using NamedPipesServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultithreadedPipeServer
{
    internal class Program
    {
        private static string pipeName = "Demo2Pipe";
        static void Main(string[] args)
        {
            Run();
        }
        public static void Run()
        {
            Task.Run(() => Server());

            Task.Delay(5000).Wait();

            var clients = new List<string>()
            {
                "Client 1",
                "Client 2",
                "Client 3",
                "Client 4",
                "Client 5",
                "Client 6",
                "Client 7",
                "Client 8"
            };
            Parallel.ForEach(clients, (c) => Client(c));
        }
        static void Server()
        {
            var server = new NamedPipeServer(pipeName, 20);
            server.NewRequestEvent += (s, e) =>
            {
                e.Response = "Echo. " + e.Request;
            };
            Task.Delay(1000000).Wait();
            server.Dispose();
        }
        static void Client(string clientName)
        {
            using (var client = new NamedPipeClient(pipeName))
            {
                var request = clientName + " Request a";
                var response = client.SendRequest(request);
                Console.WriteLine(response);
                Task.Delay(100).Wait();

                var request1 = clientName + " Request b";
                var response1 = client.SendRequest(request1);
                Console.WriteLine(response1);
                Task.Delay(100).Wait();

                var request2 = clientName + " Request c";
                var response2 = client.SendRequest(request2);
                Console.WriteLine(response2);
            }
        }
    }
}
