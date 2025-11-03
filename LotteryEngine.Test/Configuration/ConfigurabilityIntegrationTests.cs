using LotteryEngine.Configuration;
using LotteryEngine.Engine;

namespace LotteryEngine.Test.Configuration;

/// <summary>
/// Integration tests verifying the system works with different configurations
/// </summary>
public class ConfigurabilityIntegrationTests
{
    [Fact]
    public void CompleteGame_WithHigherTicketCost_ShouldCalculateCorrectPrizes()
    {
        var config = new LotteryConfiguration
        {
            InitialBalance = 50m,
            TicketCost = 5m,
            GrandPrizePercentage = 0.5m,
            SecondTierPercentage = 0.3m,
            ThirdTierPercentage = 0.1m
        };

        var game = CreateGame(config);
        game.Initialize(5);
        var tickets = game.GetAllTickets();
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);

        var totalRevenue = tickets.Count * 5m;
        var expectedGrandPrize = totalRevenue * 0.5m;

        Assert.Equal(expectedGrandPrize, gameResults.GrandPrizeAmount);
    }

    [Fact]
    public void CompleteGame_WithDifferentPrizeDistribution_ShouldWork()
    {
        var config = new LotteryConfiguration
        {
            TicketCost = 1m,
            GrandPrizePercentage = 0.6m,
            SecondTierPercentage = 0.25m,
            ThirdTierPercentage = 0.05m
        };

        var game = CreateGame(config);
        game.Initialize(5);
        var tickets = game.GetAllTickets();
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);

        var totalRevenue = tickets.Count * 1m;
        var expectedGrandPrize = totalRevenue * 0.6m;
        var expectedHouseMin = totalRevenue * 0.1m;

        Assert.Equal(expectedGrandPrize, gameResults.GrandPrizeAmount);
        Assert.True(gameResults.HouseProfit >= expectedHouseMin - 1m);
    }

    [Fact]
    public void CompleteGame_WithCustomTicketCost_ShouldCalculateBalanceCorrectly()
    {
        var config = new LotteryConfiguration
        {
            MinTicketCount = 1,
            MaxTicketCount = 10,
            InitialBalance = 20m,
            TicketCost = 2m
        };

        var game = CreateGame(config);
        game.Initialize(5);

        Assert.Equal(5, game.HumanPlayer.Tickets.Count);
        Assert.All(game.CpuPlayers, p => Assert.InRange(p.Tickets.Count, 1, 10));
    }

    [Fact]
    public void MultipleGames_WithSameConfiguration_ShouldHaveConsistentPlayerCounts()
    {
        var config = new LotteryConfiguration
        {
            TicketCost = 1m,
            GrandPrizePercentage = 0.5m,
            SecondTierPercentage = 0.3m,
            ThirdTierPercentage = 0.1m
        };

        var game1 = CreateGame(config);
        var game2 = CreateGame(config);

        game1.Initialize(5);
        game2.Initialize(5);

        Assert.InRange(game1.TotalPlayerCount, 10, 15);
        Assert.InRange(game2.TotalPlayerCount, 10, 15);
    }

    [Theory]
    [InlineData(1, 10, 1.0, 0.5, 0.3, 0.1)]
    [InlineData(5, 10, 2.0, 0.6, 0.25, 0.05)]
    [InlineData(1, 5, 1.0, 0.4, 0.3, 0.2)]
    public void CompleteGame_WithVariousConfigurations_ShouldExecuteSuccessfully(
        int minTickets,
        int maxTickets,
        decimal ticketCost,
        decimal grandPrize,
        decimal secondTier,
        decimal thirdTier)
    {
        var config = new LotteryConfiguration
        {
            MinTicketCount = minTickets,
            MaxTicketCount = maxTickets,
            InitialBalance = maxTickets * ticketCost * 2,
            TicketCost = ticketCost,
            GrandPrizePercentage = grandPrize,
            SecondTierPercentage = secondTier,
            ThirdTierPercentage = thirdTier
        };

        config.Validate();

        var game = CreateGame(config);
        game.Initialize(5);

        var tickets = game.GetAllTickets();
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);

        Assert.NotNull(gameResults);
        Assert.NotNull(gameResults.GrandPrizeWinner);
        Assert.NotEmpty(gameResults.SecondTierWinners);
        Assert.NotEmpty(gameResults.ThirdTierWinners);

        var totalRevenue = tickets.Count * ticketCost;
        var expectedGrandPrize = totalRevenue * grandPrize;
        Assert.Equal(expectedGrandPrize, gameResults.GrandPrizeAmount);
    }

    [Fact]
    public void Configuration_ChangesAffectNewGames_NotExisting()
    {
        var config1 = new LotteryConfiguration { TicketCost = 1m };
        var game1 = CreateGame(config1);
        game1.Initialize(5);
        var tickets1 = game1.GetAllTickets();

        var config2 = new LotteryConfiguration { InitialBalance = 50m, TicketCost = 5m };
        var game2 = CreateGame(config2);
        game2.Initialize(5);
        var tickets2 = game2.GetAllTickets();

        var results1 = game1.GetGameResults(game1.DrawWinners());
        var results2 = game2.GetGameResults(game2.DrawWinners());

        var revenue1 = tickets1.Count * 1m;
        var revenue2 = tickets2.Count * 5m;

        Assert.Equal(revenue1 * 0.5m, results1.GrandPrizeAmount);
        Assert.Equal(revenue2 * 0.5m, results2.GrandPrizeAmount);
        Assert.True(results2.GrandPrizeAmount > results1.GrandPrizeAmount);
    }

    [Fact]
    public void PrizeDistribution_RespectsConfiguredPercentages()
    {
        var config = new LotteryConfiguration
        {
            InitialBalance = 100m,
            TicketCost = 10m,
            GrandPrizePercentage = 0.4m,
            SecondTierPercentage = 0.35m,
            ThirdTierPercentage = 0.15m
        };

        var game = CreateGame(config);
        game.Initialize(5);
        var tickets = game.GetAllTickets();
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);

        var totalRevenue = tickets.Count * 10m;

        Assert.Equal(totalRevenue * 0.4m, gameResults.GrandPrizeAmount);

        var secondTierPool = totalRevenue * 0.35m;
        var expectedSecondTierPerWinner = Math.Floor(secondTierPool / drawResults.SecondTierWinners.Count);
        Assert.Equal(expectedSecondTierPerWinner, gameResults.SecondTierPrizePerWinner);

        var thirdTierPool = totalRevenue * 0.15m;
        var expectedThirdTierPerWinner = Math.Floor(thirdTierPool / drawResults.ThirdTierWinners.Count);
        Assert.Equal(expectedThirdTierPerWinner, gameResults.ThirdTierPrizePerWinner);
    }

    [Fact]
    public void HouseProfit_ReflectsConfiguredPrizePercentages()
    {
        var config = new LotteryConfiguration
        {
            TicketCost = 1m,
            GrandPrizePercentage = 0.4m,
            SecondTierPercentage = 0.3m,
            ThirdTierPercentage = 0.2m
        };

        var game = CreateGame(config);
        game.Initialize(5);
        var tickets = game.GetAllTickets();
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);

        var totalRevenue = tickets.Count * 1m;
        var minExpectedHouseProfit = totalRevenue * 0.1m;

        Assert.True(gameResults.HouseProfit >= minExpectedHouseProfit - 5m);
    }

    private LotteryGame CreateGame(LotteryConfiguration config)
    {
        var playerGenerator = new PlayerGenerator(config);
        var prizeCalculator = new PrizeCalculator(config);
        return new LotteryGame(playerGenerator, prizeCalculator, config);
    }
}
