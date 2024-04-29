﻿using TaskSearcher2.Utilities;

namespace TaskSearcher2
{
    internal static class FolderSearch
    {
        public static void Run()
        {
            Console.Write("Keyword? ");
            var name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.Error.WriteLine("Keyword is not provided");
                return;
            }

            Console.Write("Skip special folders (eg. bin, dist)? ");
            var skipText = Console.ReadLine() ?? "";
            var skipValue = skipText == ""
                || "y".Equals(skipText, StringComparison.OrdinalIgnoreCase)
                || "true".Equals(skipText, StringComparison.OrdinalIgnoreCase);

            Console.WriteLine($"Start folder search for {name} {(skipValue ? "with" : "without")} skipping special folders");

            IEnumerable<string> currentPaths = Directory
                .EnumerateDirectories(TasksFolder.Path)
                .OrderByDescending(t => t);

            var level = 0;

            while (currentPaths.Any()
                && level <= 100)
            {
                var nextablePaths = new List<string>();

                foreach (var currentPath in currentPaths)
                {
                    ProgressConsole.Current($"L{level + 1}: {currentPath}");

                    if (currentPath.Contains(name, StringComparison.OrdinalIgnoreCase))
                    {
                        using (new ColorConsoleScope(ConsoleColor.Yellow))
                        {
                            ProgressConsole.Line(currentPath);
                        }
                    }
                    else
                    {
                        if (!skipValue
                            || !SkipNames.Contains(currentPath))
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
