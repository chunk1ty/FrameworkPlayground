using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Server;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var response = await client.SayHelloAsync(new HelloRequest { Name = "World", RequestNumber = 1 });

            Console.WriteLine($"Greeting: [{response.Message}] response number: [{response.ResponseNumber}]");
        }
    }
}