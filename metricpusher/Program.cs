using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Threading.Tasks;

namespace metricpusher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = "InstrumentationKey=2a56f42a-89b7-43ed-9e09-ca851523b16a;IngestionEndpoint=https://westus2-2.in.applicationinsights.azure.com/"; // demo1
            config.TelemetryInitializers.Add(new MyRoleInstanceInitialier());
            config.TelemetryInitializers.Add(new MyRoleNameInitialier());
            config.DefaultTelemetrySink.TelemetryProcessorChainBuilder.Use((next) => new AutocollectedMetricsExtractor(next));
            config.DefaultTelemetrySink.TelemetryProcessorChainBuilder.Use((next) => new TelemetryDropper(next));
            config.DefaultTelemetrySink.TelemetryProcessorChainBuilder.Build();
            var client = new TelemetryClient(config);
            PushMetric(client);
        }

        private static void PushMetric(TelemetryClient telemetryClient)
        {
            string[] types = new string[] { "type1", "type2", "type3", "type4", "type5" };
            string[] resCodes = new string[] { "200", "500", "503", "401", "429" };
            string[] targets = new string[] { "targetA", "targetB", "targetC", "targetD", "targetE", "targetF", "targetG", "targetH", "targetI" };
            SeverityLevel[] levels = new SeverityLevel[] { SeverityLevel.Critical, SeverityLevel.Error, SeverityLevel.Information, SeverityLevel.Verbose, SeverityLevel.Warning };
            Random rand = new Random();

            while (true)
            {
                telemetryClient.TrackRequest("MYReq", DateTimeOffset.Now, TimeSpan.FromMilliseconds(rand.Next(1, 10000)),
                     resCodes[rand.Next(0, 5)], (rand.Next(1, 10) < 4) ? true : false);

                telemetryClient.TrackDependency(types[rand.Next(0, 4)],
                    targets[rand.Next(0, 9)], "depName", "data", DateTimeOffset.Now,
                    TimeSpan.FromMilliseconds(rand.Next(1, 10000)), resCodes[rand.Next(0, 5)], (rand.Next(1, 10) < 4) ? true : false);
                telemetryClient.TrackException(new Exception("Something bad"));
                telemetryClient.TrackTrace("Hello message", levels[rand.Next(0, 5)]);

                Task.Delay(20).Wait();
            }
        }

        internal class TelemetryDropper : ITelemetryProcessor
        {
            private ITelemetryProcessor next;

            public TelemetryDropper(ITelemetryProcessor next)
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

        internal class MyRoleInstanceInitialier : ITelemetryInitializer
        {
            public void Initialize(ITelemetry telemetry)
            {
                telemetry.Context.Cloud.RoleInstance = "MyRoleInstance";
            }
        }

        internal class MyRoleNameInitialier : ITelemetryInitializer
        {
            string[] roles = new string[] { "RoleNameA", "RoleNameB" };
            Random rand = new Random();

            public void Initialize(ITelemetry telemetry)
            {
                telemetry.Context.Cloud.RoleName = roles[rand.Next(0, 2)];
            }
        }
    }
}
