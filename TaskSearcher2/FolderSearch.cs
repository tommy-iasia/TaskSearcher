using TaskSearcher2.Utilities;

namespace TaskSearcher2
{
    internal static class FolderSearch
    {
        public static void Run()
        {
            Console.Write("Keyword? ");
            var targetName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(targetName))
            {
                Console.Error.WriteLine("Keyword is not provided");
                return;
            }

            Console.Write("Skip special folders (eg. bin, dist)? ");
            var skipText = Console.ReadLine() ?? "";
            var skipValue = skipText == ""
                || "y".Equals(skipText, StringComparison.OrdinalIgnoreCase)
                || "true".Equals(skipText, StringComparison.OrdinalIgnoreCase);

            Console.WriteLine($"Start folder search for {targetName} {(skipValue ? "with" : "without")} skipping special folders");

            var currentPaths = Directory
                .EnumerateDirectories(TasksFolder.Path)
                .OrderByDescending(t => t)
                .ToList();

            var level = 0;

            while (currentPaths.Any()
                && level <= 100)
            {
                var nextablePaths = new List<string>();

                foreach (var currentPath in currentPaths)
                {
                    ProgressConsole.Current($"L{level + 1}: {currentPath}");

                    var currentName = Path.GetFileName(currentPath);
                    if (currentName.Contains(targetName, StringComparison.OrdinalIgnoreCase))
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

                currentPaths.Clear();
                foreach (var nextablePath in nextablePaths)
                {
                    try
                    {
                        var innerPaths = Directory.EnumerateDirectories(nextablePath);
                        currentPaths.AddRange(innerPaths);
                    }
                    catch
                    {
                        using (new ColorConsoleScope(ConsoleColor.Gray))
                        {
                            ProgressConsole.Line($"L{level + 1}: Fail to dive into {nextablePath}");
                        }
                    }
                }

                level++;
            }

            ProgressConsole.Line($"Finished search at L{level + 1}");
        }

        public static string[] SkipNames { get; } = new[]
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
