namespace LotteryEngine.Entities;

/// <summary>
/// Represents the final game results with winners and prize amounts
/// </summary>
public class GameResults
{
    public string GrandPrizeWinner { get; set; } = string.Empty;
    public decimal GrandPrizeAmount { get; init; }
    public Dictionary<string, int> SecondTierWinners { get; set; } = new();
    public decimal SecondTierPrizePerWinner { get; init; }
    public Dictionary<string, int> ThirdTierWinners { get; set; } = new();
    public decimal ThirdTierPrizePerWinner { get; init; }
    public decimal HouseProfit { get; set; }
}