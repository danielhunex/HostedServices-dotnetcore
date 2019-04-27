using System;
using System.Threading.Tasks;

namespace HostedService
{
    public class StrategyC : IStrategy
    {
        public StrategyC()
        {
            Console.WriteLine("...StrategyC Created...");
        }
        public async Task ExecuteAsync()
        {
            await Task.Run(() => Console.WriteLine("StrategyC: Executing"));
        }
    }
}