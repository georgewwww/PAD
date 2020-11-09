namespace WebServer.Application.Abstractions
{
    public interface IActionResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
