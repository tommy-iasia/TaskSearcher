using TaskSearcher2.Utilities;

namespace TaskSearcher2
{
    internal static class FileSearch
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

            Console.WriteLine($"Start file search for {targetName} {(skipValue ? "with" : "without")} skipping special folders");

            var currentFolders = Directory
                .EnumerateDirectories(TasksFolder.Path)
                .OrderByDescending(t => t)
                .ToList();

            var level = 0;

            while (currentFolders.Any()
                && level <= 100)
            {
                var nextableFolders = new List<string>();

                foreach (var currentFolder in currentFolders)
                {
                    var currentFolderName = Path.GetFileName(currentFolder);
                    if (!skipValue
                        || !FolderSearch.SkipNames.Contains(currentFolderName))
                    {
                        ProgressConsole.Current($"L{level + 1}: {currentFolder}");

                        var filePaths = Directory.EnumerateFiles(currentFolder);
                        foreach (var filePath in filePaths)
                        {
                            var fileName = Path.GetFileName(filePath);
                            if (fileName.Contains(targetName, StringComparison.OrdinalIgnoreCase))
                            {
                                using (new ColorConsoleScope(ConsoleColor.Yellow))
                                {
                                    ProgressConsole.Line(filePath);
                                }
                            }
                        }

                        nextableFolders.Add(currentFolder);
                    }
                }

                currentFolders.Clear();
                foreach (var nextableFolder in nextableFolders)
                {
                    try
                    {
                        var innerFolders = Directory.EnumerateDirectories(nextableFolder);
                        currentFolders.AddRange(innerFolders);
                    }
                    catch
                    {
                        using (new ColorConsoleScope(ConsoleColor.Gray))
                        {
                            ProgressConsole.Line($"L{level + 1}: Fail to dive into {nextableFolder}");
                        }
                    }
                }

                level++;
            }

            ProgressConsole.Line($"Finished search at L{level + 1}");
        }
    }
}
