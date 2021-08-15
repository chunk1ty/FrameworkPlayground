using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TplDataFlow
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var consumer1 = new Consumer(1, "Andriyan");
            var consumer2 = new Consumer(2, "Kiro");

            Producer producer = new Producer();
            producer.BroadcastBlock.LinkTo(consumer1.ActionBlock, message => message.Id % 2 == 0);
            producer.BroadcastBlock.LinkTo(consumer2.ActionBlock, message => message.Id % 3 == 0);

            for (int i = 0; i < 20; i++)
            {
                await producer.Publish(new Message(i, $"data {i}"));
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
