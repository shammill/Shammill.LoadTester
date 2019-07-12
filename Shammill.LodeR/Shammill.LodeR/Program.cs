using Shammill.LodeR.InputHandler;
using System;

/// <summary>
/// LodeR - Only the cleverest names for my apps.
/// </summary>
namespace Shammill.LodeR
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
