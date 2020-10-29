using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace SmartProxy.Helpers
{
    public static class CopyHelper
    {
        public static readonly int BufferSize = 1024 * 1024 * 5; // 5mb buffer;

        public static void Headers(NameValueCollection nameValueCollection, NameValueCollection webHeaderCollection)
        {
            foreach (string requestHeader in nameValueCollection)
            {
                webHeaderCollection.Add(requestHeader, nameValueCollection[requestHeader]);
            }
        }

        public static void InputStream(WebRequest webRequest, HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return;
            }
            using (Stream body = request.InputStream) // here we have data
            {
                using (StreamReader reader = new StreamReader(body, request.ContentEncoding))
                {
                    var readToEnd = reader.ReadToEnd();

                    using (var outStream = webRequest.GetRequestStream())
                    using (StreamWriter writer = new StreamWriter(outStream, request.ContentEncoding))
                    {
                        writer.Write(readToEnd);
                    }
                }
            }
        }

        public static void Response(HttpListenerResponse httpListenerResponse, byte[] buffer, int size)
        {
            httpListenerResponse.OutputStream.Write(buffer, 0, size);
        }

        public static void RequestDetails(WebRequest webRequest, HttpListenerRequest request)
        {
            webRequest.Method = request.HttpMethod;
            webRequest.ContentLength = request.ContentLength64;
            webRequest.ContentType = request.ContentType;
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }
            return dict;
        }

        public static NameValueCollection ToNameValueCollection(this Dictionary<string, string> dictionary)
        {
            var nameValueCollection = new NameValueCollection();
            foreach (var keyValuePair in dictionary)
            {
                nameValueCollection.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return nameValueCollection;
        }
    }
}
