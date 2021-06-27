using Shammill.LoadTester.InputHandler;
using System;

namespace Shammill.LoadTester
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Ready.");
                Input.Process();
            }
        }
    }
}
