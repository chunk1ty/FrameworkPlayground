using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // var channel = GrpcChannel.ForAddress("https://localhost:5001");
            // var channel = new Channel("localhost:5001", ChannelCredentials.Insecure);
            // var client = new Calculator.CalculatorClient(channel);
            //
            // var response = await client.AdditionAsync(new AdditionRequest() {FirstNumber = 1, SecondNumber = 2});
            //
            // Console.WriteLine(response.Result);

            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Calculator.CalculatorClient(channel);
            var response = await client.AdditionAsync(new AdditionRequest() {FirstNumber = 1, SecondNumber = 2});
            Console.WriteLine($"Result: [{response.Result}]");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}