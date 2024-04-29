using TaskSearcher2;

Console.WriteLine("Task Searcher");

Console.WriteLine("1. JIRA");
Console.WriteLine("2. Folder");
Console.WriteLine("3. File");
Console.WriteLine("4. Text");

Console.Write("What to search? ");
string? typeText = Console.ReadLine();

if (!int.TryParse(typeText, out var typeValue))
{
    Console.Error.WriteLine($"{typeText} is not a number");
    return;
}

switch (typeValue)
{
    case 1:
        await JiraSearch.Run();
        break;

    case 2:
        FolderSearch.Run();
        break;

    case 3:
        FileSearch.Run();
        break;

    case 4:
        TextSearch.Run();
        break;

    default:
        Console.Error.WriteLine($"{typeValue} is invalid");
        return;
}

Console.WriteLine("Program ends");