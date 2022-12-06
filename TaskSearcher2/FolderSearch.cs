using TaskSearcher2.Utilities;

namespace TaskSearcher2
{
    internal static class FolderSearch
    {
        public static void Run()
        {
            Console.Write("Keyword? ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.Error.WriteLine("Keyword is not provided");
                return;
            }

            Console.Write("Skip special folders (eg. bin, dist)? ");
            string? skipText = Console.ReadLine() ?? "";

            var skipValue = 
                "y".Equals(skipText, StringComparison.OrdinalIgnoreCase)
                || "true".Equals(skipText, StringComparison.OrdinalIgnoreCase);

            Console.WriteLine($"Start folder search for {name} {(skipValue ? "with" : "without")} skipping special folders");

            var currentPaths = Directory.EnumerateDirectories(TasksFolder.Path);
            var level = 0;

            while (currentPaths.Any())
            {
                var nextablePaths = new List<string>();

                foreach (var currentPath in currentPaths)
                {
                    ProgressConsole.Current($"L{level + 1}: {currentPath}");

                    var currentName = Path.GetFileName(currentPath);
                    if (currentName.Contains(name, StringComparison.OrdinalIgnoreCase))
                    {
                        using (new ColorConsoleScope(ConsoleColor.Yellow))
                        {
                            ProgressConsole.Line(currentPath);
                        }
                    }
                    else
                    {
                        if (!skipValue
                            || !SkipNames.Contains(currentName))
                        {
                            nextablePaths.Add(currentPath);
                        }
                    }
                }

                currentPaths = nextablePaths
                    .SelectMany(path => Directory.EnumerateDirectories(path));
                level++;
            }

            ProgressConsole.Line("Finished search");
        }

        private static string[] SkipNames { get; } = new[]
        {
            ".svn", ".git",
            "bin", "obj",
            "classes", "test-classes",
            "node_modules", "dist",
            "iAsiaLogs",
            ".idea",
        };
    }
}
