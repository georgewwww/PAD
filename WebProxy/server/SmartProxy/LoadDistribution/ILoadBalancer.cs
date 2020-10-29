using System;

namespace SmartProxy.LoadDistribution
{
    public interface ILoadBalancer
    {
        Uri Next();
        void Add(Uri uri);
    }
}
