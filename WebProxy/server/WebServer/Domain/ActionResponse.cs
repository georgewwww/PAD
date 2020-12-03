using WebServer.Application.Abstractions;

namespace WebServer.Domain
{
    public class ActionResponse : IActionResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
