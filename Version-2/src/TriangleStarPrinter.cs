using System;

namespace HostedService
{
    public class TriangleStarPrinter : IStarPrinter
    {
        public void Print()
        {
            for (int i = 1; i < 10; i++)
            {
                for (int j = 10; j > i; j--)
                {
                    Console.Write(" ");
                }
                for (int z = 0; z < 2 * i; z++)
                {
                    Console.Write("*");
                }
                Console.WriteLine();

            }
        }
    }
}