using LotteryEngine.Configuration;
using LotteryEngine.Entities;
using LotteryEngine.Interfaces;

namespace LotteryEngine.Engine;

/// <summary>
/// Handles prize distribution and winner selection for the lottery
/// </summary>
public class PrizeCalculator : IPrizeCalculator
{
    private const decimal SecondTierWinnerPercentage = 0.10m;
    private const decimal ThirdTierWinnerPercentage = 0.20m;

    private readonly LotteryConfiguration _config;
    private readonly Random _random = new();

    public PrizeCalculator(LotteryConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Calculates the total revenue from all tickets
    /// </summary>
    /// <param name="tickets">The list of all tickets sold</param>
    /// <returns>Total revenue</returns>
    public decimal CalculateTotalRevenue(List<Ticket> tickets)
    {
        return tickets.Count * _config.TicketCost;
    }

    /// <summary>
    /// Calculates the grand prize amount (50% of total revenue)
    /// </summary>
    /// <param name="tickets">The list of all tickets sold</param>
    /// <returns>Grand prize amount</returns>
    public decimal CalculateGrandPrize(List<Ticket> tickets)
    {
        return CalculateTotalRevenue(tickets) * _config.GrandPrizePercentage;
    }

    /// <summary>
    /// Calculates the second tier prize pool (30% of total revenue)
    /// </summary>
    /// <param name="tickets">The list of all tickets sold</param>
    /// <returns>Second tier prize pool</returns>
    public decimal CalculateSecondTierPool(List<Ticket> tickets)
    {
        return CalculateTotalRevenue(tickets) * _config.SecondTierPercentage;
    }

    /// <summary>
    /// Calculates the third tier prize pool (10% of total revenue)
    /// </summary>
    /// <param name="tickets">The list of all tickets sold</param>
    /// <returns>Third tier prize pool</returns>
    public decimal CalculateThirdTierPool(List<Ticket> tickets)
    {
        return CalculateTotalRevenue(tickets) * _config.ThirdTierPercentage;
    }

    /// <summary>
    /// Calculates the house profit (remaining revenue after all prizes)
    /// </summary>
    /// <param name="tickets">The list of all tickets sold</param>
    /// <returns>House profit</returns>
    public decimal CalculateHouseProfit(List<Ticket> tickets)
    {
        var totalRevenue = CalculateTotalRevenue(tickets);
        var totalPrizes = CalculateGrandPrize(tickets) +
                         CalculateSecondTierPool(tickets) +
                         CalculateThirdTierPool(tickets);
        return totalRevenue - totalPrizes;
    }

    /// <summary>
    /// Draws winners for all prize tiers
    /// </summary>
    /// <param name="tickets">The list of all tickets sold</param>
    /// <returns>Drawing results with winners for each tier</returns>
    public DrawResults DrawWinners(List<Ticket> tickets)
    {
        var results = new DrawResults();
        var availableTickets = new List<Ticket>(tickets);

        // Draw grand prize winner (1 ticket)
        var grandPrizeIndex = _random.Next(availableTickets.Count);
        results.GrandPrizeWinners.Add(availableTickets[grandPrizeIndex]);
        availableTickets.RemoveAt(grandPrizeIndex);

        // Draw second tier winners (10% of total tickets)
        var secondTierWinnerCount = (int)Math.Floor(tickets.Count * SecondTierWinnerPercentage);
        for (var i = 0; i < secondTierWinnerCount; i++)
        {
            var winnerIndex = _random.Next(availableTickets.Count);
            results.SecondTierWinners.Add(availableTickets[winnerIndex]);
            availableTickets.RemoveAt(winnerIndex);
        }

        // Draw third tier winners (20% of total tickets)
        var thirdTierWinnerCount = (int)Math.Floor(tickets.Count * ThirdTierWinnerPercentage);
        for (var i = 0; i < thirdTierWinnerCount; i++)
        {
            var winnerIndex = _random.Next(availableTickets.Count);
            results.ThirdTierWinners.Add(availableTickets[winnerIndex]);
            availableTickets.RemoveAt(winnerIndex);
        }

        return results;
    }

    /// <summary>
    /// Calculates the prize amount for each second tier winner
    /// </summary>
    /// <param name="tickets">The list of all tickets sold</param>
    /// <param name="winnerCount">Number of second tier winners</param>
    /// <returns>Prize amount per winner</returns>
    public decimal CalculateSecondTierPrizePerWinner(List<Ticket> tickets, int winnerCount)
    {
        if (winnerCount == 0) return 0;
        var pool = CalculateSecondTierPool(tickets);
        return Math.Floor(pool / winnerCount);
    }

    /// <summary>
    /// Calculates the prize amount for each third tier winner
    /// </summary>
    /// <param name="tickets">The list of all tickets sold</param>
    /// <param name="winnerCount">Number of third tier winners</param>
    /// <returns>Prize amount per winner</returns>
    public decimal CalculateThirdTierPrizePerWinner(List<Ticket> tickets, int winnerCount)
    {
        if (winnerCount == 0) return 0;
        var pool = CalculateThirdTierPool(tickets);
        return Math.Floor(pool / winnerCount);
    }
}
