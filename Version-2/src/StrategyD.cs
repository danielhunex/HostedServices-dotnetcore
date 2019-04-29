using System;
using System.Threading.Tasks;

namespace HostedService
{
    public class StrategyD : IStrategy
    {
        private readonly IStarPrinter _starPrinter;
        public StrategyD(IStarPrinter starPrinter)
        {
            Console.WriteLine("...StrategyD Created...");
            _starPrinter = starPrinter;
        }
        public async Task ExecuteAsync()
        {
            await Task.Run(() => _starPrinter.Print());
        }
    }
}