using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TplDataFlow
{
    public class Producer
    {
        private readonly BroadcastBlock<Message> _broadcastBlock;

        public Producer()
        {
            _broadcastBlock = new BroadcastBlock<Message>(_ => _);
        }

        public BroadcastBlock<Message> BroadcastBlock => _broadcastBlock;

        public async Task<bool> Publish(Message message)
        {
            return await _broadcastBlock.SendAsync(message);
        }
    }
}
