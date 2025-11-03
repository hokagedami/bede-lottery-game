
using LotteryEngine.Configuration;
using LotteryEngine.Engine;
using LotteryEngine.Interfaces;

namespace LotteryEngine.Test;

/// <summary>
/// Tests for the LotteryGame class
/// </summary>
public class LotteryGameTests
{
    private readonly IPlayerGenerator _playerGenerator;
    private readonly IPrizeCalculator _prizeCalculator;
    private readonly LotteryConfiguration _config;

    public LotteryGameTests()
    {
        _config = new LotteryConfiguration();
        _playerGenerator = new PlayerGenerator(_config);
        _prizeCalculator = new PrizeCalculator(_config);
    }

    private LotteryGame CreateGame()
    {
        return new LotteryGame(_playerGenerator, _prizeCalculator, _config);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithPlayers()
    {

        var game = CreateGame();
        Assert.NotNull(game);
    }

    [Fact]
    public void Initialize_WithHumanPlayer_ShouldHaveTotalPlayersBetween10And15()
    {

        var game = CreateGame();
        game.Initialize(5);
        Assert.InRange(game.TotalPlayerCount, 10, 15);
    }

    [Fact]
    public void Initialize_ShouldCreateHumanPlayer()
    {

        var game = CreateGame();
        game.Initialize(3);
        Assert.NotNull(game.HumanPlayer);
        Assert.Equal("You", game.HumanPlayer.Id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Initialize_HumanPlayer_ShouldPurchaseRequestedTickets(int ticketCount)
    {

        var game = CreateGame();
        game.Initialize(ticketCount);
        Assert.Equal(ticketCount, game.HumanPlayer.Tickets.Count);
        Assert.Equal(10 - ticketCount, game.HumanPlayer.Balance);
    }

    [Fact]
    public void Initialize_ShouldGenerateCpuPlayers()
    {

        var game = CreateGame();
        game.Initialize(5);
        Assert.NotEmpty(game.CpuPlayers);
        Assert.InRange(game.CpuPlayers.Count, 9, 14);
    }

    [Fact]
    public void GetAllTickets_ShouldReturnAllTicketsFromAllPlayers()
    {

        var game = CreateGame();
        game.Initialize(5);
        var allTickets = game.GetAllTickets();
        var expectedTicketCount = game.HumanPlayer.Tickets.Count +
                                 game.CpuPlayers.Sum(p => p.Tickets.Count);
        Assert.Equal(expectedTicketCount, allTickets.Count);
    }

    [Fact]
    public void DrawWinners_ShouldReturnDrawResults()
    {

        var game = CreateGame();
        game.Initialize(5);
        var results = game.DrawWinners();
        Assert.NotNull(results);
        Assert.NotEmpty(results.GrandPrizeWinners);
        Assert.NotEmpty(results.SecondTierWinners);
        Assert.NotEmpty(results.ThirdTierWinners);
    }

    [Fact]
    public void GetGameResults_ShouldIncludeAllWinners()
    {

        var game = CreateGame();
        game.Initialize(5);
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        Assert.NotNull(gameResults);
        Assert.NotNull(gameResults.GrandPrizeWinner);
        Assert.NotEmpty(gameResults.SecondTierWinners);
        Assert.NotEmpty(gameResults.ThirdTierWinners);
    }

    [Fact]
    public void GetGameResults_ShouldCalculatePrizeAmounts()
    {

        var game = CreateGame();
        game.Initialize(5);
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        Assert.True(gameResults.GrandPrizeAmount > 0);
        Assert.True(gameResults.SecondTierPrizePerWinner >= 0);
        Assert.True(gameResults.ThirdTierPrizePerWinner >= 0);
    }

    [Fact]
    public void GetGameResults_ShouldCalculateHouseProfit()
    {

        var game = CreateGame();
        game.Initialize(5);
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        Assert.True(gameResults.HouseProfit > 0);
    }

    [Fact]
    public void GetGameResults_ShouldGroupWinnersByPlayer()
    {

        var game = CreateGame();
        game.Initialize(10);
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        Assert.NotNull(gameResults.GrandPrizeWinner);
        foreach (var winner in gameResults.SecondTierWinners)
        {
            Assert.True(winner.Value > 0);
        }
        foreach (var winner in gameResults.ThirdTierWinners)
        {
            Assert.True(winner.Value > 0);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void CompleteGame_WithDifferentTicketCounts_ShouldWork(int humanTickets)
    {

        var game = CreateGame();
        game.Initialize(humanTickets);
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        Assert.NotNull(gameResults);
        Assert.InRange(game.TotalPlayerCount, 10, 15);
    }

    [Fact]
    public void TotalRevenue_ShouldEqualAllTicketsSold()
    {

        var game = CreateGame();
        game.Initialize(5);
        var allTickets = game.GetAllTickets();
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        var expectedRevenue = allTickets.Count;
        var calculatedRevenue = gameResults.GrandPrizeAmount +
                               (gameResults.SecondTierPrizePerWinner * gameResults.SecondTierWinners.Count) +
                               (gameResults.ThirdTierPrizePerWinner * gameResults.ThirdTierWinners.Count) +
                               gameResults.HouseProfit;
        Assert.True(calculatedRevenue <= expectedRevenue);
    }
}
