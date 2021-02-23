using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace standardmetricdemo
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
            var aiOptions = new ApplicationInsightsServiceOptions();
            // aiOptions.ConnectionString = "InstrumentationKey=457e0aaf-fd2a-4e37-8df1-18c95bb66ca3;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/";
            aiOptions.ConnectionString = "InstrumentationKey=37b3dc54-9164-46b5-9b59-2d690c6fb409;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/";
            aiOptions.EnableHeartbeat = false;
            aiOptions.EnablePerformanceCounterCollectionModule = false;
            services.AddApplicationInsightsTelemetry(aiOptions);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "standardmetricdemo", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TelemetryConfiguration tc)
        {
            var builder = tc.DefaultTelemetrySink.TelemetryProcessorChainBuilder;
            builder.Use((next) => new DropAllButMetricsProcessor(next));
            builder.Build();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "standardmetricdemo v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    internal class DropAllButMetricsProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor next;
        public DropAllButMetricsProcessor(ITelemetryProcessor next)
        {
            this.next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is MetricTelemetry)
            {
                this.next.Process(item);
            }
            else
            {
                return;
            }
        }
    }
}
