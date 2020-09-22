using System.Collections.Concurrent;
using Broker.Models;

namespace Broker.Infrastructure.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ConcurrentQueue<Message> _messages;

        public MessageRepository()
        {
            _messages = new ConcurrentQueue<Message>();
        }

        public void Add(Message message)
        {
            _messages.Enqueue(message);
        }

        public Message GetNext()
        {
            _messages.TryDequeue(out var message);
            return message;
        }

        public bool IsEmpty()
        {
            return _messages.IsEmpty;
        }
    }
}
