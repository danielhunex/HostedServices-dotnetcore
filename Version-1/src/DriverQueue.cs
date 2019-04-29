using System;
using System.Collections;
using System.Collections.Generic;

namespace HostedService
{
    public class DriverQueue
    {
        private static Queue<Type> _queue = new Queue<Type>();

        static DriverQueue()
        {
            _queue.Enqueue(typeof(StrategyA));
            _queue.Enqueue(typeof(StrategyC));
            _queue.Enqueue(typeof(StrategyB));
            _queue.Enqueue(typeof(StrategyC));
            _queue.Enqueue(typeof(StrategyA));
            _queue.Enqueue(typeof(StrategyC));
            _queue.Enqueue(typeof(StrategyA));
            _queue.Enqueue(typeof(StrategyC));
        }

        public static Type TryDequeue()
        {
            Type _;
            if (_queue.TryDequeue(out _) == false)
            {
                throw new InvalidOperationException();
            }
            return _;
        }
    }
}