using System.Diagnostics;
using Chiral.Console.Exceptions;

namespace Chiral.Console.Utilities;

public static class ProcessUtility
{
    public static CancellationToken CreateCancellationToken()
    {
        var source = new CancellationTokenSource();

        System.Console.CancelKeyPress += delegate {  EraseCancelKeyPressCharacters(); };

        AppDomain.CurrentDomain.ProcessExit += delegate { source.Cancel(); };

        return source.Token;
    }

    public static string GetExecutionPath()
    {
        if (Debugger.IsAttached)
        {
            return Path.GetFullPath(".");
        }

        var process = Process.GetCurrentProcess();
        var path = Path.GetDirectoryName(process.MainModule?.FileName);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new PathNotFoundException();
        }

        return path;
    }

    #region Private Methods

    private static void EraseCancelKeyPressCharacters()
    {
        ConsoleUtility.ClearCharacters(2);
    }

    #endregion
}
