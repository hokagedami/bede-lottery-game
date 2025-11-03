using LotteryEngine.Entities;
using LotteryEngine.Interfaces;

namespace LotteryEngine.Test.Helpers;

/// <summary>
/// Custom prize calculator for testing extensibility
/// </summary>
public class CustomPrizeCalculator : IPrizeCalculator
{
    public DrawResults DrawWinners(List<Ticket> allTickets)
    {
        return new DrawResults
        {
            GrandPrizeWinners = [allTickets[0]],
            SecondTierWinners = allTickets.Skip(1).Take(2).ToList(),
            ThirdTierWinners = allTickets.Skip(3).Take(3).ToList()
        };
    }

    public decimal CalculateGrandPrize(List<Ticket> allTickets) => 100m;

    public decimal CalculateSecondTierPrizePerWinner(List<Ticket> allTickets, int winnerCount) => 50m;

    public decimal CalculateThirdTierPrizePerWinner(List<Ticket> allTickets, int winnerCount) => 25m;

    public decimal CalculateHouseProfit(List<Ticket> allTickets) => 10m;
}