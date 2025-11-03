namespace LotteryEngine.Entities;

/// <summary>
/// Represents a lottery ticket
/// </summary>
public class Ticket
{
    private static int _nextId = 1;

    /// <summary>
    /// Gets the unique identifier for this ticket
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the player who owns this ticket
    /// </summary>
    public Player Owner { get; }

    /// <summary>
    /// Initializes a new instance of the Ticket class
    /// </summary>
    /// <param name="owner">The player who owns this ticket</param>
    public Ticket(Player owner)
    {
        Id = _nextId++;
        Owner = owner;
    }
}
