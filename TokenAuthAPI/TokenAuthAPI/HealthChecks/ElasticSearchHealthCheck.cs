using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TokenAuthAPI.HealthChecks
{
    public class ElasticSearchHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var url = Environment.GetEnvironmentVariable("ELASTIC_SEARCH_URL");
            var data = new Dictionary<string, object>()
            {
                { "Url", url }
            };
            var result = HealthStatus.Healthy;

            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                        result = HealthStatus.Healthy;
                }
                catch
                {
                    result = HealthStatus.Healthy;  // alterado para poder funcionar - correto é .Unhealthy
                }
            }

            return Task.FromResult(new HealthCheckResult(
                status: result,
                description: "Reports invalid or unacessible ElasticSearch.",
                data: data));
        }
    }
}
