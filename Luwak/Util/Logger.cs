using System.Text;

namespace WebServerProgram;

public static class Logger
{
    public enum Level
    {
        Verbose,
        Error,
    }

    public static void Log(string message, Level level = Level.Verbose)
    {
        StringBuilder messageBuilder = new StringBuilder();
        messageBuilder.Append(DateTime.Now);
        string prefix = " [Info] ";
        if (level == Level.Error)
        {
            prefix = " [Error] ";
        }
        messageBuilder.Append(prefix);
        messageBuilder.Append(message);
        Console.WriteLine(messageBuilder);
    }
}