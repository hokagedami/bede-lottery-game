namespace LotteryEngine.Entities;

/// <summary>
/// Represents a player in the lottery game
/// </summary>
public class Player
{
    private readonly decimal _ticketCost;
    private readonly int _minTicketCount;
    private readonly int _maxTicketCount;

    /// <summary>
    /// Gets the player's unique identifier
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the player's current balance
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Gets the list of tickets owned by this player
    /// </summary>
    public List<Ticket> Tickets { get; }

    /// <summary>
    /// Initializes a new instance of the Player class
    /// </summary>
    /// <param name="id">The unique identifier for the player</param>
    /// <param name="initialBalance">The starting balance for the player</param>
    /// <param name="ticketCost">The cost per ticket</param>
    /// <param name="minTicketCount">Minimum number of tickets that can be purchased</param>
    /// <param name="maxTicketCount">Maximum number of tickets that can be purchased</param>
    public Player(string id, decimal initialBalance, decimal ticketCost = 1m, int minTicketCount = 1, 
        int maxTicketCount = 10)
    {
        Id = id;
        Balance = initialBalance;
        _ticketCost = ticketCost;
        _minTicketCount = minTicketCount;
        _maxTicketCount = maxTicketCount;
        Tickets = [];
    }

    /// <summary>
    /// Purchases the specified number of tickets for this player
    /// </summary>
    /// <param name="ticketCount">The number of tickets to purchase</param>
    /// <exception cref="ArgumentException">Thrown when ticket count is outside the valid range</exception>
    /// <exception cref="InvalidOperationException">Thrown when the player doesn't have enough balance</exception>
    public void PurchaseTickets(int ticketCount)
    {
        if (ticketCount < _minTicketCount || ticketCount > _maxTicketCount)
        {
            throw new ArgumentException(
                $"Ticket count must be between {_minTicketCount} and {_maxTicketCount}",
                nameof(ticketCount));
        }

        var totalCost = ticketCount * _ticketCost;
        if (totalCost > Balance)
        {
            throw new InvalidOperationException(
                $"Insufficient balance. Required: ${totalCost}, Available: ${Balance}");
        }

        Balance -= totalCost;

        for (var i = 0; i < ticketCount; i++)
        {
            var ticket = new Ticket(this);
            Tickets.Add(ticket);
        }
    }
}
