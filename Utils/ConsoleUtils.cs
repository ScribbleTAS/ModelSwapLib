using MelonLoader;

namespace ModelSwapLib;

internal static class ConsoleUtils
{
    internal static void Msg(string message)
    {
        Melon<Core>.Logger.Msg(message);
    }

    internal static void Warning(string message)
    {
        Melon<Core>.Logger.Warning(message);
    }
    
    internal static void Error(string message)
    {
        Melon<Core>.Logger.Error(message);
    }

    internal static void BigError(string message)
    {
        Melon<Core>.Logger.BigError(message);
    }
}