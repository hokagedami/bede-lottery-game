namespace LotteryEngine.Entities;

/// <summary>
/// Represents the results of a lottery drawing
/// </summary>
public class DrawResults
{
    public List<Ticket> GrandPrizeWinners { get; set; } = [];
    public List<Ticket> SecondTierWinners { get; set; } = [];
    public List<Ticket> ThirdTierWinners { get; set; } = [];
}