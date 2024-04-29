using System.Text;
using TaskSearcher2.Utilities;

namespace TaskSearcher2
{
    internal static class TextSearch
    {
        public static void Run()
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

            IEnumerable<string> currentFolders = Directory
                .EnumerateDirectories(TasksFolder.Path)
                .OrderByDescending(t => t);

            var level = 0;

            while (currentFolders.Any()
                && level <= 100)
            {
                var nextableFolders = new List<string>();

                foreach (var currentFolder in currentFolders)
                {
                    var filePaths = Directory.EnumerateFiles(currentFolder);
                    foreach (var filePath in filePaths)
                    {
                        if (skipExtensionValue
                            && SkipExtensions.Any(extension => filePath.EndsWith(extension)))
                        {
                            continue;
                        }

                        ProgressConsole.Current($"L{level + 1}: {filePath}");

                        var fileText = File.ReadAllText(filePath);
                        if (fileText.Contains(targetText, StringComparison.OrdinalIgnoreCase))
                        {
                            using (new ColorConsoleScope(ConsoleColor.Yellow))
                            {
                                ProgressConsole.Line(filePath);
                            }
                        }
                    }

                    if (!skipFolderValue
                        || !FolderSearch.SkipNames.Contains(currentFolder))
                    {
                        nextableFolders.Add(currentFolder);
                    }
                }

                currentFolders = nextableFolders
                    .SelectMany(path => Directory.EnumerateDirectories(path));
                level++;
            }

            ProgressConsole.Line("Finished search");
        }

        private static string[] SkipExtensions { get; } = new[]
        {
            ".7z", ".rar", ".zip", ".gz",
            ".png", ".jpg", ".bmp",
            ".exe"
        };
    }
}
