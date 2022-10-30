using NamedPipesServer;
using System;
using System.Threading.Tasks;

namespace PipeClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client("client 1");
        }
        static void Client(string clientName)
        {
            using var client = new NamedPipeClient("Demo2Pipe");
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
