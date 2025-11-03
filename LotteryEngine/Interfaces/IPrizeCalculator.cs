using LotteryEngine.Entities;

namespace LotteryEngine.Interfaces;

/// <summary>
/// Interface for calculating lottery prizes
/// </summary>
public interface IPrizeCalculator
{
    /// <summary>
    /// Draws winners from all tickets
    /// </summary>
    /// <param name="allTickets">All tickets in the lottery</param>
    /// <returns>The draw results with winners</returns>
    DrawResults DrawWinners(List<Ticket> allTickets);

    /// <summary>
    /// Calculates the grand prize amount
    /// </summary>
    /// <param name="allTickets">All tickets in the lottery</param>
    /// <returns>Grand prize amount in dollars</returns>
    decimal CalculateGrandPrize(List<Ticket> allTickets);

    /// <summary>
    /// Calculates the second tier prize per winner
    /// </summary>
    /// <param name="allTickets">All tickets in the lottery</param>
    /// <param name="winnerCount">Number of second tier winners</param>
    /// <returns>Prize amount per winner in dollars</returns>
    decimal CalculateSecondTierPrizePerWinner(List<Ticket> allTickets, int winnerCount);

    /// <summary>
    /// Calculates the third tier prize per winner
    /// </summary>
    /// <param name="allTickets">All tickets in the lottery</param>
    /// <param name="winnerCount">Number of third tier winners</param>
    /// <returns>Prize amount per winner in dollars</returns>
    decimal CalculateThirdTierPrizePerWinner(List<Ticket> allTickets, int winnerCount);

    /// <summary>
    /// Calculates the house profit
    /// </summary>
    /// <param name="allTickets">All tickets in the lottery</param>
    /// <returns>House profit in dollars</returns>
    decimal CalculateHouseProfit(List<Ticket> allTickets);
}
