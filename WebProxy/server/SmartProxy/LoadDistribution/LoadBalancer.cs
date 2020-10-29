using System;
using System.Collections.Concurrent;

namespace SmartProxy.LoadDistribution
{
    public class LoadBalancer : ILoadBalancer
    {
        private readonly ConcurrentQueue<Uri> uris = new ConcurrentQueue<Uri>();

        public void Add(Uri uri)
        {
            uris.Enqueue(uri);
        }

        public Uri Next()
        {
            if (uris.TryDequeue(out var res))
            {
                uris.Enqueue(res);
                return res;
            }
            return null;
        }
    }
}
