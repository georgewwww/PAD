using System.Collections.Generic;

namespace SmartProxy.RedisCache
{
    public class CachedResponse
    {
        public Dictionary<string, string> Headers { get; set; }
        public byte[] Body { get; set; }
    }
}
