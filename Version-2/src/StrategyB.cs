using System;
using System.Threading.Tasks;

namespace HostedService
{
    public class StrategyB : IStrategy
    {
        public StrategyB()
        {
            Console.WriteLine("...StrategyB Created...");
        }
        public async Task ExecuteAsync()
        {
            await Task.Run(() => Console.WriteLine("StrategyB: Executing"));
        }
    }
}