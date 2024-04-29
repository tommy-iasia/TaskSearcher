using TaskSearcher2.Utilities;

namespace TaskSearcher2
{
    internal static class TextSearch
    {
        public static async Task RunAsync()
        {
            Console.Write("Text? ");
            var targetText = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(targetText))
            {
                Console.Error.WriteLine("Text is not provided");
                return;
            }

            Console.Write("Skip special folders (eg. bin, dist)? ");
            var skipFolderText = Console.ReadLine() ?? "";
            var skipFolderValue = skipFolderText == ""
                || "y".Equals(skipFolderText, StringComparison.OrdinalIgnoreCase)
                || "true".Equals(skipFolderText, StringComparison.OrdinalIgnoreCase);

            Console.Write("Skip special file extensions (eg. 7z, zip)? ");
            var skipExtensionText = Console.ReadLine() ?? "";
            var skipExtensionValue = skipExtensionText == ""
                || "y".Equals(skipExtensionText, StringComparison.OrdinalIgnoreCase)
                || "true".Equals(skipExtensionText, StringComparison.OrdinalIgnoreCase);

            Console.WriteLine($"Start text search for {targetText} with "
                + $"special folders {(skipFolderValue ? "skipped" : "kept")} and "
                + $"special extensions {(skipExtensionValue ? "skipped" : "kept")}");

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
                    if (!skipFolderValue
                        || !FolderSearch.SkipNames.Contains(currentFolderName))
                    {
                        ProgressConsole.Current($"L{level + 1}: {currentFolder}");

                        var filePaths = Directory.EnumerateFiles(currentFolder);
                        foreach (var filePath in filePaths)
                        {
                            if (skipExtensionValue
                                && SkipExtensions.Any(extension => filePath.EndsWith(extension)))
                            {
                                continue;
                            }

                            ProgressConsole.Current($"L{level + 1}: {filePath}");

                            string fileText;
                            try
                            {
                                fileText = await File.ReadAllTextAsync(filePath);
                            }
                            catch
                            {
                                using (new ColorConsoleScope(ConsoleColor.Gray))
                                {
                                    ProgressConsole.Line($"L{level + 1}: fail to read {filePath}");
                                }

                                continue;
                            }

                            if (fileText.Contains(targetText, StringComparison.OrdinalIgnoreCase))
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

        private static string[] SkipExtensions { get; } = new[]
        {
            ".7z", ".rar", ".zip", ".gz",
            ".png", ".jpg", ".bmp",
            ".exe"
        };
    }
}
