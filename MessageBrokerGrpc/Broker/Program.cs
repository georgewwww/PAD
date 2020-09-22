using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Broker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // To trust a certificate -> dotnet dev-certs https --trust
            CreateHostBuilder(args).Build().Run();

            //var worker = new Worker();
            //Task.Factory.StartNew(worker.DoSendMessageWork, TaskCreationOptions.LongRunning);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
