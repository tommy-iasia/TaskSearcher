using TaskSearcher2;

Console.WriteLine("Task Searcher");

Console.WriteLine("1. JIRA");
Console.WriteLine("2. Folder");
Console.WriteLine("3. File");
Console.WriteLine("4. Text");

Console.Write("What to search? ");
var text = Console.ReadLine() ?? "";

switch (text.Trim().ToLower())
{
    case "1":
    case "jira":
        await JiraSearch.RunAsync();
        break;

    case "2":
    case "folder":
        FolderSearch.Run();
        break;

    case "3":
    case "file":
        FileSearch.Run();
        break;

    case "4":
    case "text":
        await TextSearch.RunAsync();
        break;

    default:
        Console.Error.WriteLine($"{text} is invalid");
        return;
}

Console.WriteLine("Program ends");