using System;
using System.Threading.Tasks;

namespace Dispatcher
{
    public class Dispatcher
    {
        public event EventHandler<Message> MessageReceived;

        public Dispatcher(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public void OnMessageReceived(Message message)
        {
            if (MessageReceived == null)
            {
                return;
            }

            var delegates = MessageReceived.GetInvocationList();
            Parallel.ForEach(delegates, d => d.DynamicInvoke(this, message));

            // MessageReceived.Invoke(this, message);
        }
    }
}