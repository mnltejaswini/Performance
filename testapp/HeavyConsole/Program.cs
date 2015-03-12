using System;

namespace FatConsole
{
    public class Program
    {
        public int Main(string[] args)
        {
#if DNX451
            System.Console.WriteLine("Hello, World! This is DNX451.");
#else
            System.Console.WriteLine("Hello, World! This is DNXCORE50.");
#endif

            return 0;
        }
    }
}
