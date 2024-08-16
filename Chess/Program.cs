using System.Diagnostics;

namespace Chess
{
    internal static class Program
    {
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new GameWindow());
        }
    }
}