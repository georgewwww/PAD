using Common;
using System.Collections.Concurrent;

namespace Broker
{
    public static class PayloadStorage
    {
        private static ConcurrentQueue<Payload> _payloadQueue;

        static PayloadStorage()
        {
            _payloadQueue = new ConcurrentQueue<Payload>();
        }

        public static void Add(Payload payload)
        {
            _payloadQueue.Enqueue(payload);
        }

        public static Payload GetNext()
        {
            _payloadQueue.TryDequeue(out Payload payload);

            return payload;
        }

        public static bool IsEmpty()
        {
            return _payloadQueue.IsEmpty;
        }
    }
}
