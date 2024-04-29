using TaskSearcher2.Utilities;

namespace TaskSearcher2
{
    internal static class FileSearch
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

            Console.WriteLine($"Start file search for {name} {(skipValue ? "with" : "without")} skipping special folders");

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
                    ProgressConsole.Current($"L{level + 1}: {currentFolder}");

                    var files = Directory.EnumerateFiles(currentFolder);
                    foreach (var file in files)
                    {
                        if (file.Contains(name, StringComparison.OrdinalIgnoreCase))
                        {
                            using (new ColorConsoleScope(ConsoleColor.Yellow))
                            {
                                ProgressConsole.Line(currentFolder);
                            }
                        }
                    }

                    if (!skipValue
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
    }
}
