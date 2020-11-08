using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using WebServer.Application;
using WebServer.Application.Abstractions;
using WebServer.Application.Services;
using WebServer.Infrastructure.gRPC;
using WebServer.Infrastructure.Persistence;
using WebServer.Infrastructure.Repository;

namespace WebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appId = Guid.NewGuid();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<MessageBusConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseHealthCheck(provider);
                    cfg.Host(new Uri($"rabbitmq://{Configuration.GetConnectionString("MessageBrokerHost")}"), h =>
                    {
                        h.Username(Configuration.GetConnectionString("MessageBrokerUsername"));
                        h.Password(Configuration.GetConnectionString("MessageBrokerPassword"));
                    });
                    cfg.ReceiveEndpoint(appId.ToString(), ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<MessageBusConsumer>(provider);
                    });
                }));
            });
            services.AddMassTransitHostedService();

            services.AddControllers();

            services.Configure<ConnectionDatabaseSettings>(Configuration.GetSection("ConnectionDatabaseSettings"));
            services.AddSingleton<IConnectionDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<ConnectionDatabaseSettings>>().Value);

            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<IActorService, ActorService>();

            services.AddSingleton<IApplicationDbContext, ApplicationDbContext>();
            services.AddSingleton<IActorRepository, ActorSyncRepository>();
            services.AddSingleton<IMovieRepository, MovieSyncRepository>();

            services.AddSingleton<IMessageBrokerServiceClient, MessageBrokerServiceClient>();
            services.AddSingleton(new ServerDescriptor(appId));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                Task.Factory.StartNew(() => EnqueueServer(app));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to send Up notification to Message Broker: {e}");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        private async void EnqueueServer(IApplicationBuilder app)
        {
            var messageBrokerClient = app.ApplicationServices.GetService<IMessageBrokerServiceClient>();
            var serviceDescriptor = app.ApplicationServices.GetService<ServerDescriptor>();
            serviceDescriptor.Url = GetServerAddress(app);

            await messageBrokerClient.Subscribe(serviceDescriptor.Id.ToString(), serviceDescriptor.Url);
        }

        private string GetServerAddress(IApplicationBuilder app)
        {
            System.Threading.Thread.Sleep(2000);

            try
            {
                var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
                var address = serverAddressesFeature.Addresses.First();
                return address;
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Unable to send notification to Load Balancer {e}"); ;
            }

            return string.Empty;
        }
    }
}
