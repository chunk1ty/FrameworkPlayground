using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dispatcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var dispatcher = new Dispatcher(1);
            Consumer consumer1 = new Consumer(1, "A");
            consumer1.Subscribe(dispatcher);
            Consumer consumer2 = new Consumer(2, "B");
            // consumer2.Subscribe(consumersDispatcher);
            Consumer consumer3 = new Consumer(3, "C");

            var consumersDispatcher1 = new Dispatcher(2);


            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
                if (i % 2 == 0)
                {
                    var message = new Message(1, $"data {i}");
                    var message2 = new Message(2, $"data {i + 200}");
                    var message3 = new Message(3, $"data {i + 300}");

                    //tasks.Add(Task.Run(() => consumersDispatcher.OnMessageReceived(message)));
                    //tasks.Add(Task.Run(() => consumersDispatcher.OnMessageReceived(message2)));
                    //tasks.Add(Task.Run(() => consumersDispatcher.OnMessageReceived(message3)));
                    dispatcher.OnMessageReceived(message);
                    dispatcher.OnMessageReceived(message2);
                    dispatcher.OnMessageReceived(message3);
                    continue;
                }

                //var message1 = new Message(2, $"data {i}");
                //consumersDispatcher1.OnMessageReceived(message1);
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine(sw.Elapsed);
        }
    }
}