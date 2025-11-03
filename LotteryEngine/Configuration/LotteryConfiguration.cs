namespace LotteryEngine.Configuration;

/// <summary>
/// Configuration settings for the lottery system
/// </summary>
public class LotteryConfiguration
{
    /// <summary>
    /// Minimum number of tickets a player can purchase
    /// </summary>
    public int MinTicketCount { get; set; } = 1;

    /// <summary>
    /// Maximum number of tickets a player can purchase
    /// </summary>
    public int MaxTicketCount { get; set; } = 10;

    /// <summary>
    /// Initial balance for all players
    /// </summary>
    public decimal InitialBalance { get; set; } = 10m;

    /// <summary>
    /// Cost of each lottery ticket
    /// </summary>
    public decimal TicketCost { get; set; } = 1m;

    /// <summary>
    /// Percentage of total revenue allocated to grand prize (0.0 to 1.0)
    /// </summary>
    public decimal GrandPrizePercentage { get; set; } = 0.5m;

    /// <summary>
    /// Percentage of total revenue allocated to second tier prizes (0.0 to 1.0)
    /// </summary>
    public decimal SecondTierPercentage { get; set; } = 0.3m;

    /// <summary>
    /// Percentage of total revenue allocated to third tier prizes (0.0 to 1.0)
    /// </summary>
    public decimal ThirdTierPercentage { get; set; } = 0.1m;

    /// <summary>
    /// Number of tickets eligible for second tier (top X%)
    /// </summary>
    public int SecondTierWinnerCount { get; set; } = 10;

    /// <summary>
    /// Number of tickets eligible for third tier (top X%)
    /// </summary>
    public int ThirdTierWinnerCount { get; set; } = 20;

    /// <summary>
    /// Validates the configuration settings
    /// </summary>
    public void Validate()
    {
        if (MinTicketCount < 1)
            throw new ArgumentException("MinTicketCount must be at least 1");

        if (MaxTicketCount < MinTicketCount)
            throw new ArgumentException("MaxTicketCount must be greater than or equal to MinTicketCount");

        if (TicketCost <= 0)
            throw new ArgumentException("TicketCost must be greater than 0");

        if (GrandPrizePercentage + SecondTierPercentage + ThirdTierPercentage > 1.0m)
            throw new ArgumentException("Total prize percentages cannot exceed 100%");
    }
}
