using Lox;

switch (args.Length)
{
    case > 1:
        Console.WriteLine($"Usage: CSLox [script]");
        Environment.Exit(64);
        break;
    case 1:
        RunFile(args[0]);
        break;
    default:
        RunPrompt();
        break;
}

return;

// Runs the file from command line
void RunFile(string fileName)
{
    var data = File.ReadAllText(Path.GetFullPath(fileName));
    Run(data);
    if(LoxLog.HasErrors())
    {
        Environment.Exit(65);
    }
}

// If we start the program without argument to write code here
void RunPrompt()
{
    while (true)
    {
        Console.Write("#> ");
        var line = Console.ReadLine();

        if (string.IsNullOrEmpty(line))
        {
            continue;
        }

        if(line.Equals("LoxExit", StringComparison.InvariantCultureIgnoreCase))
        {
            break;
        }

        Run(line);
    }
}

void Run(string source)
{
    var scanner = new Scanner(source);
    var tokens = scanner.ScanTokens();

    foreach (var token in tokens)
    {
        Console.WriteLine(token);
    }
}



