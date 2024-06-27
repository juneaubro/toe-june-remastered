public static class EventColors
{
    /// <summary>
    /// Add color to the beginning of <see cref="EventType"/> ouput
    /// </summary>
    /// <param name="type"><see cref="EventType"/> to format console output for</param>
    public static void FormatEventTypeOutput(EventType type)
    {
        string eventString = $"{type.ToString()}Event";

        switch (type)
        {
            case EventType.Join:
                Format(eventString, ConsoleColor.Green, ConsoleColor.DarkGreen);
                break;
            case EventType.Leave:
                Format(eventString, ConsoleColor.Red, ConsoleColor.DarkRed);
                break;
            case EventType.UpdateRotation:
                Format(eventString, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
                break;
            case EventType.UpdateLocation:
                Format(eventString, ConsoleColor.Yellow, ConsoleColor.DarkYellow);
                break;
            case EventType.StartGame:
                Format(eventString, ConsoleColor.Magenta, ConsoleColor.DarkMagenta);
                break;
            case EventType.EndGame:
                Format(eventString, ConsoleColor.Magenta, ConsoleColor.DarkMagenta);
                break;
            case EventType.KickPlayer:
                Format(eventString, ConsoleColor.Red, ConsoleColor.DarkRed);
                break;
            default: break;
        }
    }

    private static void Format(string eventString, ConsoleColor bracketColor, ConsoleColor eventColor, bool resetColor = true)
    {
        Console.ForegroundColor = bracketColor;
        Console.Write("[");
        Console.ForegroundColor = eventColor;
        Console.Write(eventString);
        Console.ForegroundColor = bracketColor;
        Console.Write("] ");

        if(resetColor)
            Console.ResetColor();
    }
}