using System;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MassTransitRoutingSlipMonitoring
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

                busConfigurator.AddActivitiesFromNamespaceContaining<SaveOrderActivity>();

                busConfigurator.AddRequestClient<SubmitOrder>(
                    new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost");

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMassTransitHostedService();

            services.AddControllers();
            
            services.AddOpenApiDocument(cfg => 
                cfg.PostProcess = d => d.Info.Title = "MassTransit Routing Slip Monitoring API");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(); // serve Swagger UI

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // The readiness check uses all registered checks with the 'ready' tag.
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"),
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    // Exclude all checks and return a 200-Ok.
                    Predicate = _ => false
                });
            });
        }
    }
}