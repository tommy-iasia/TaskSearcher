using TaskSearcher2.Utilities;

namespace TaskSearcher2
{
    public static class JiraSearch
    {
        public static async Task Run()
        {
            Console.Write("JIRA code? ");
            string? code = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(code))
            {
                Console.Error.WriteLine("JIRA code is not provided");
                return;
            }

            Console.WriteLine($"Start JIRA search for {code}");

            var directoryPaths = Directory.GetDirectories(TasksFolder.Path);
            foreach (var (directoryPath, index) in directoryPaths.WithIndex())
            {
                ProgressConsole.Current($"{directoryPath} ({index + 1}/{directoryPaths.Length})");

                var directoryName = Path.GetFileName(directoryPath);
                if (directoryName.Contains(code, StringComparison.OrdinalIgnoreCase))
                {
                    using (new ColorConsoleScope(ConsoleColor.Yellow))
                    {
                        ProgressConsole.Line(directoryPath);
                    }
                }
                else
                {
                    var linkPaths = Directory.EnumerateFiles(directoryPath, "*.url");
                    foreach (var linkPath in linkPaths)
                    {
                        var linkText = await File.ReadAllTextAsync(linkPath);
                        if (linkText.Contains(code, StringComparison.OrdinalIgnoreCase))
                        {
                            var linkName = Path.GetFileName(linkPath);

                            using (new ColorConsoleScope(ConsoleColor.Yellow))
                            {
                                ProgressConsole.Line(@$"{directoryPath}\{linkName}");
                            }
                        }
                    }
                }
            }

            ProgressConsole.Line("Finished search");
        }
    }
}
