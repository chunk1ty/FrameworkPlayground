using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dispatcher
{
    public class Consumer
    {
        private readonly ICollection<Dispatcher> _dispatchers;

        public Consumer(int id, string name)
        {
            Id = id;
            Name = name;
            _dispatchers = new List<Dispatcher>();
        }

        public int Id { get; }

        public string Name { get; }

        public void Subscribe(Dispatcher dispatcher)
        {
            if (_dispatchers.Any(m => m.Id == dispatcher.Id))
            {
                return;
            }

            _dispatchers.Add(dispatcher);
            dispatcher.MessageReceived += Foo;
        }

        private void Foo(object sender, Message message)
        {
            // process message
            Console.WriteLine($"{DateTime.Now} | Consumer: {Name} {Id} | Message: {message.Id} {message.Data} |#thread {Thread.CurrentThread.ManagedThreadId}");

            Thread.Sleep(5 * 1000);
        }
    }
}