using Newtonsoft.Json;
using SmartProxy.Helpers;
using SmartProxy.RedisCache;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace SmartProxy.LoadDistribution
{
    public class LoadBalancerListener
    {
        private readonly HttpListener httpListener;
        private readonly ILoadBalancer loadBalancer;
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public LoadBalancerListener(HttpListener httpListener, ILoadBalancer loadBalancer, IConnectionMultiplexer connectionMultiplexer)
        {
            this.httpListener = httpListener;
            this.loadBalancer = loadBalancer;
            this.connectionMultiplexer = connectionMultiplexer;
        }

        public void Listen()
        {
            RemoveAllCache();
            httpListener.Prefixes.Add(Environment.GetEnvironmentVariable("LoadBalancerListenUrl"));

            Console.WriteLine("Listening to following profiles:");
            foreach (var prefix in httpListener.Prefixes)
            {
                Console.WriteLine($"\t{prefix}");
            }

            httpListener.Start();
            httpListener.BeginGetContext(ProcessRequestCallback, null);
        }

        public void Stop()
        {
            httpListener.Stop();
        }

        private void ProcessRequestCallback(IAsyncResult ar)
        {
            var context = httpListener.EndGetContext(ar);

            // Tell listener to stast the next connection simultaneously
            httpListener.BeginGetContext(ProcessRequestCallback, null);

            var request = context.Request;
            var requestUrl = new Uri(request.Url.ToString().ToLower());

            // Check for double request
            if (requestUrl.AbsoluteUri.Contains("favicon")) return;

            if (request.HttpMethod == "OPTIONS")
            {
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Z-Key");
                context.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
                context.Response.Close();
                return;
            }

            var readingResource = request.HttpMethod == "GET";
            var pathList = requestUrl.LocalPath.Split("/").Where(s => !string.IsNullOrEmpty(s)).Take(2).ToList();

            if (pathList.Count == 2)
            {
                if (!readingResource)
                {
                    var resource = pathList[0] + "/" + pathList[1];
                    RemoveCacheByPattern("*" + resource + "*");
                    Console.Out.WriteLineAsync($"[Thread {Thread.CurrentThread.ManagedThreadId}] Removing cache for " + resource + " because of method : " + request.HttpMethod);
                }
            }
            var redisKey = requestUrl.PathAndQuery;

            if (readingResource &&
                connectionMultiplexer.GetDatabase(0).KeyExists(redisKey))
            {
                Console.Out.WriteLineAsync($"[Thread {Thread.CurrentThread.ManagedThreadId}] Returning cached request.");
                var content = connectionMultiplexer.GetDatabase(0).StringGet(redisKey);
                var cachedResponse = JsonConvert.DeserializeObject<CachedResponse>(content);

                CopyHelper.Headers(CopyHelper.ToNameValueCollection(cachedResponse.Headers), context.Response.Headers);
                CopyHelper.Response(context.Response, cachedResponse.Body, cachedResponse.Body.Length);
            }
            else
            {
                var uri = GetRedirectUri(request);

                Console.Out.WriteLineAsync($"[Thread {Thread.CurrentThread.ManagedThreadId}] Incoming {request.HttpMethod} request for: {uri}");

                var webRequest = WebRequest.Create(uri);
                CopyHelper.RequestDetails(webRequest, request);
                CopyHelper.Headers(request.Headers, webRequest.Headers);
                CopyHelper.InputStream(webRequest, request);
                var webResponse = webRequest.GetResponse();

                var buffer = new byte[CopyHelper.BufferSize];
                var read = webResponse.GetResponseStream().Read(buffer, 0, buffer.Length);

                CopyHelper.Headers(webResponse.Headers, context.Response.Headers);
                CopyHelper.Response(context.Response, buffer, read);

                if (readingResource)
                {
                    var serializeObject = JsonConvert.SerializeObject(new CachedResponse
                    {
                        Body = buffer.Take(read).ToArray(),
                        Headers = CopyHelper.ToDictionary(webResponse.Headers)
                    });
                    connectionMultiplexer.GetDatabase(0).StringSet(redisKey, serializeObject);
                }
            }
        }

        private Uri GetRedirectUri(HttpListenerRequest request)
        {
            var redirectUriBuilder = new UriBuilder(request.Url);
            var redirectUri = loadBalancer.Next();

            if (redirectUri == null) return null;

            redirectUriBuilder.Port = redirectUri.Port;
            redirectUriBuilder.Host = redirectUri.Host;

            return redirectUriBuilder.Uri;
        }

        private void RemoveCacheByPattern(string pattern)
        {
            foreach (var ep in connectionMultiplexer.GetEndPoints())
            {
                var server = connectionMultiplexer.GetServer(ep);
                var redisKeys = server.Keys(pattern: pattern);
                foreach (var key in redisKeys)
                {
                    connectionMultiplexer.GetDatabase(0).KeyDelete(key);
                }
            }
        }

        private void RemoveAllCache()
        {
            foreach (var ep in connectionMultiplexer.GetEndPoints(true))
            {
                var server = connectionMultiplexer.GetServer(ep);
                server.FlushAllDatabases();
            }
        }
    }
}
