﻿using Broker.Infrastructure.Repository;
using Broker.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Broker
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            // Adding persistance injections
            services.AddSingleton<IMessageRepository, MessageRepository>();
            services.AddSingleton<IConnectionRepository, ConnectionRepository>();
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
