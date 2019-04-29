using System;
using System.Threading.Tasks;

namespace HostedService
{
    public class StrategyA : IStrategy
    {
        public StrategyA()
        {
            Console.WriteLine("...StrategyA Created...");
        }
        public async Task ExecuteAsync()
        {
            await Task.Run(() => Console.WriteLine("StrategyA: Executing"));
        }
    }
}