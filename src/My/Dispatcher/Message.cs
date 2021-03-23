using System;

namespace Dispatcher
{
    public class Message : EventArgs
    {
        public Message(int id, string data)
        {
            Id = id;
            Data = data;
        }

        public int Id { get; }

        public string Data { get; }
    }
}