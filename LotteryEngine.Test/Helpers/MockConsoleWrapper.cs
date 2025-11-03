using LotteryEngine.Interfaces;

namespace LotteryEngine.Test.Helpers;

/// <summary>
/// Mock console wrapper for testing
/// </summary>
public class MockConsoleWrapper : IConsoleWrapper
{
    private readonly Queue<string> _inputQueue = new();
    public List<string> Output { get; } = [];

    public void WriteLine(string message)
    {
        Output.Add(message);
    }

    public void Write(string message)
    {
        Output.Add(message);
    }

    public string? ReadLine()
    {
        return _inputQueue.Count > 0 ? _inputQueue.Dequeue() : null;
    }

    public void QueueInput(string input)
    {
        _inputQueue.Enqueue(input);
    }
}