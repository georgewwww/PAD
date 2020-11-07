using MessageBroker;
using MessageBroker.Models;
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
using WebServer.Application.Abstractions.Domain;
using WebServer.Application.Services;
using WebServer.Domain.Events;
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
            services.AddSingleton(new ServerDescriptor());
            services.AddSingleton(new MessageBrokerReceiver(Configuration.GetConnectionString("MessageBroker")));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                Task.Factory.StartNew(() => EnqueueServer(app));
                StartSync(app);
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

        private void StartSync(IApplicationBuilder app)
        {
            var messageBrokerReceiver = app.ApplicationServices.GetService<MessageBrokerReceiver>();
            var serverDescriptor = app.ApplicationServices.GetService<ServerDescriptor>();
            var actorRepository = app.ApplicationServices.GetService<IActorRepository>();
            var movieRepository = app.ApplicationServices.GetService<IMovieRepository>();

            Console.WriteLine("> Registering synchronizer");
            messageBrokerReceiver.Subscribe(serverDescriptor.Id.ToString(), actorRepository, movieRepository);
        }

        private async void EnqueueServer(IApplicationBuilder app)
        {
            var messageBroker = app.ApplicationServices.GetService<IMessageBrokerServiceClient>();
            var serviceDescriptor = app.ApplicationServices.GetService<ServerDescriptor>();
            serviceDescriptor.Url = GetServerAddress(app);

            await messageBroker.Subscribe(serviceDescriptor.Id.ToString(), serviceDescriptor.Url);
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
