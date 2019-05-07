using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostedService
{
    public class HostedServiceContext : BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public HostedServiceContext(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var type = DriverQueue.TryDequeue();
            while (type != null)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    IStrategy cmd = scope.ServiceProvider.GetRequiredService(type) as IStrategy;
                    await cmd.ExecuteAsync();
                    await Task.Delay(3000);
                }
                type = DriverQueue.TryDequeue();
            };
        }
    }
}