using Broker.Infrastructure.Persistence;
using Broker.Infrastructure.Repository;
using Broker.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Broker
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            // Adding persistence injections
            services.Configure<ConnectionDatabaseSettings>(Configuration.GetSection("ConnectionDatabaseSettings"));
            services.AddSingleton<IConnectionDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<ConnectionDatabaseSettings>>().Value);

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IConnectionRepository, ConnectionRepository>();

            services.AddHostedService<SenderWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PublisherService>();
                endpoints.MapGrpcService<SubscriberService>();
                endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Broker service started...");
                    });
            });
        }
    }
}
