using LotteryEngine.Configuration;
using LotteryEngine.Engine;
using LotteryEngine.Interfaces;
using LotteryEngine.Test.Helpers;

namespace LotteryEngine.Test;

/// <summary>
/// Integration tests for the complete application flow
/// </summary>
public class IntegrationTests
{
    private readonly IPlayerGenerator _playerGenerator;
    private readonly IPrizeCalculator _prizeCalculator;
    private readonly LotteryConfiguration _config;

    public IntegrationTests()
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
    public void CompleteGameFlow_ShouldExecuteSuccessfully()
    {

        var game = CreateGame();
        game.Initialize(5);
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        Assert.NotNull(gameResults);
        Assert.NotNull(gameResults.GrandPrizeWinner);
        Assert.NotEmpty(gameResults.SecondTierWinners);
        Assert.NotEmpty(gameResults.ThirdTierWinners);
        Assert.True(gameResults.HouseProfit > 0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void CompleteGameFlow_WithDifferentTicketCounts_ShouldWork(int humanTickets)
    {

        var game = CreateGame();
        game.Initialize(humanTickets);
        var allTickets = game.GetAllTickets();
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        Assert.InRange(game.TotalPlayerCount, 10, 15);
        Assert.Equal(humanTickets, game.HumanPlayer.Tickets.Count);
        Assert.NotNull(gameResults.GrandPrizeWinner);

        var totalRevenue = (decimal)allTickets.Count;
        var grandPrize = gameResults.GrandPrizeAmount;
        var secondTierTotal = gameResults.SecondTierPrizePerWinner * gameResults.SecondTierWinners.Values.Sum();
        var thirdTierTotal = gameResults.ThirdTierPrizePerWinner * gameResults.ThirdTierWinners.Values.Sum();
        var totalDistributed = grandPrize + secondTierTotal + thirdTierTotal + gameResults.HouseProfit;

        Assert.Equal(totalRevenue, totalDistributed);
    }

    [Fact]
    public void ConsoleInterface_CompleteFlow_ShouldWork()
    {

        var mockConsole = new MockConsoleWrapper();
        mockConsole.QueueInput("5");
        var consoleInterface = new ConsoleInterface(mockConsole);

        var game = CreateGame();
        consoleInterface.DisplayWelcome();
        var ticketCount = consoleInterface.GetTicketCount();
        game.Initialize(ticketCount);
        consoleInterface.DisplayCpuPlayers(game.CpuPlayers);
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        consoleInterface.DisplayResults(gameResults);
        Assert.NotEmpty(mockConsole.Output);
        Assert.Contains(mockConsole.Output, line => line.Contains("Welcome"));
        Assert.Contains(mockConsole.Output, line => line.Contains("Grand Prize"));
        Assert.Contains(mockConsole.Output, line => line.Contains("House"));
    }

    [Fact]
    public void PrizeDistribution_ShouldBeValid()
    {

        var game = CreateGame();
        game.Initialize(7);
        var allTickets = game.GetAllTickets();
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);
        var totalRevenue = allTickets.Count;
        Assert.Equal(totalRevenue * 0.5m, gameResults.GrandPrizeAmount);

        var allWinningPlayers = new List<string> { gameResults.GrandPrizeWinner };
        allWinningPlayers.AddRange(gameResults.SecondTierWinners.Keys);
        allWinningPlayers.AddRange(gameResults.ThirdTierWinners.Keys);

        // Note: Players can win multiple tickets in the same tier, but not across tiers
        var grandPrizeTickets = drawResults.GrandPrizeWinners;
        var secondTierTickets = drawResults.SecondTierWinners;
        var thirdTierTickets = drawResults.ThirdTierWinners;

        var allWinningTicketIds = new List<int>();
        allWinningTicketIds.AddRange(grandPrizeTickets.Select(t => t.Id));
        allWinningTicketIds.AddRange(secondTierTickets.Select(t => t.Id));
        allWinningTicketIds.AddRange(thirdTierTickets.Select(t => t.Id));

        Assert.Equal(allWinningTicketIds.Count, allWinningTicketIds.Distinct().Count());
    }

    [Fact]
    public void MultipleGames_ShouldProduceDifferentResults()
    {

        var game1 = CreateGame();
        game1.Initialize(5);
        var results1 = game1.GetGameResults(game1.DrawWinners());

        var game2 = CreateGame();
        game2.Initialize(5);
        var results2 = game2.GetGameResults(game2.DrawWinners());
        var differentResults = results1.GrandPrizeWinner != results2.GrandPrizeWinner ||
                              results1.SecondTierWinners.Count != results2.SecondTierWinners.Count ||
                              results1.ThirdTierWinners.Count != results2.ThirdTierWinners.Count;

        Assert.True(differentResults);
    }
}
