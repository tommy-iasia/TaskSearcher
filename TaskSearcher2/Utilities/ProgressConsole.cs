namespace TaskSearcher2.Utilities
{
    internal static class ProgressConsole
    {
        public static void Erase() => Write(string.Empty);

        private static void Write(string text) => Console.Write($"\r{new string(' ', Console.CursorLeft)}\r{text}");

        public static void Current(string text)
        {
            if (text.Length < Console.WindowWidth)
            {
                Write(text);
            }
            else
            {
                int front = Console.WindowWidth / 4;
                int back = Console.WindowWidth - front - 4;

                Write($"{text[..front]}...{text.Substring(text.Length - back, back)}");
            }
        }

        public static void Line(string text) => Write($"{text}\r\n");

        public static void Next() => Console.WriteLine();
    }
}
