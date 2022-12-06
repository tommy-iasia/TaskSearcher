namespace TaskSearcher2.Utilities
{
    internal class ColorConsoleScope : IDisposable
    {
        public ConsoleColor FromColor { get; }

        public ColorConsoleScope(ConsoleColor color)
        {
            FromColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
        }

        public void Dispose() => Console.ForegroundColor = FromColor;
    }
}
