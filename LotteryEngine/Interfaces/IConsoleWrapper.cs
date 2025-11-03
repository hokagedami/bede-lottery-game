namespace LotteryEngine.Interfaces;

/// <summary>
/// Abstraction for console operations to enable testing
/// </summary>
public interface IConsoleWrapper
{
    /// <summary>
    /// Writes a line to the console
    /// </summary>
    void WriteLine(string message);

    /// <summary>
    /// Writes to the console without a newline
    /// </summary>
    void Write(string message);

    /// <summary>
    /// Reads a line from the console
    /// </summary>
    string? ReadLine();
}

/// <summary>
/// Default implementation that uses System.Console
/// </summary>
public class ConsoleWrapper : IConsoleWrapper
{
    public void WriteLine(string message) => Console.WriteLine(message);
    public void Write(string message) => Console.Write(message);
    public string? ReadLine() => Console.ReadLine();
}
