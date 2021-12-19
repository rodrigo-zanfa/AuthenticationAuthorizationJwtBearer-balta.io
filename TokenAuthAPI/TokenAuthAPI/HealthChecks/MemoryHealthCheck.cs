using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TokenAuthAPI.HealthChecks
{
    public class MemoryHealthCheck : IHealthCheck
    {
        private const long THRESHOLD = 1024L * 1024L * 1024L;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var data = new Dictionary<string, object>()
            {
                { "Allocated", allocated }
            };

            var result = allocated >= THRESHOLD ? context.Registration.FailureStatus : HealthStatus.Healthy;

            return Task.FromResult(new HealthCheckResult(
                status: result,
                description: "Reports degraded status if allocated bytes >= 1 MB.",
                data: data));
        }
    }
}
