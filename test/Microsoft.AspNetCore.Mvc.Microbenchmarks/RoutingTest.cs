
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xunit.Performance;

namespace Microsoft.AspNetCore.Mvc.Microbenchmarks
{
    public class RoutingTest
    {
        public RoutingTest()
        {
        }

        [Benchmark]
        public async Task DefaultMVCRouteMatching()
        {
            var services = new ServiceCollection();

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    await Task.Delay(0);
                }
            }
        }
    }
}
