using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace TplDataFlow
{
    public class Consumer
    {
        public Consumer(int id, string name)
        {
            Id = id;
            Name = name;
            ActionBlock = new ActionBlock<Message>(Foo);
        }

        public int Id { get; }

        public string Name { get; }

        public ActionBlock<Message> ActionBlock { get; }

        private void Foo(Message message)
        {
            Console.WriteLine($"{DateTime.Now} | Consumer: {Name} {Id} | Message: {message.Id} {message.Data} |#thread {Thread.CurrentThread.ManagedThreadId}");

            Thread.Sleep(5 * 1000);
        }
    }
}
