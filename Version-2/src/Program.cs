using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostedService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton<IStarPrinter, TriangleStarPrinter>();
                    services.AddHostedService<HostedServiceContext>();
                    services.AddScoped<StrategyA>();
                    services.AddScoped<StrategyB>();
                    services.AddScoped<StrategyC>();
                    services.AddScoped<StrategyD>();
                });

            await builder.RunConsoleAsync();
        }
    }
}