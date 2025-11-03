using LotteryEngine.Configuration;
using LotteryEngine.Engine;
using LotteryEngine.Entities;
using LotteryEngine.Interfaces;
using LotteryEngine.Test.Helpers;

namespace LotteryEngine.Test;

/// <summary>
/// Tests for extensibility through interfaces and dependency injection
/// </summary>
public class ExtensibilityTests
{
    [Fact]
    public void LotteryGame_CanUseCustomPlayerGenerator()
    {
        var config = new LotteryConfiguration();
        var customGenerator = new CustomPlayerGenerator();
        var prizeCalculator = new PrizeCalculator(config);
        var game = new LotteryGame(customGenerator, prizeCalculator, config);

        game.Initialize(5);

        Assert.Equal(3, game.CpuPlayers.Count);
        Assert.All(game.CpuPlayers, p => Assert.StartsWith("CUSTOM", p.Id));
    }

    [Fact]
    public void LotteryGame_CanUseCustomPrizeCalculator()
    {
        var config = new LotteryConfiguration();
        var playerGenerator = new PlayerGenerator(config);
        var customCalculator = new CustomPrizeCalculator();
        var game = new LotteryGame(playerGenerator, customCalculator, config);

        game.Initialize(5);
        var drawResults = game.DrawWinners();
        var gameResults = game.GetGameResults(drawResults);

        Assert.Equal(100m, gameResults.GrandPrizeAmount);
        Assert.Equal(50m, gameResults.SecondTierPrizePerWinner);
        Assert.Equal(25m, gameResults.ThirdTierPrizePerWinner);
    }

    [Fact]
    public void LotteryGame_AcceptsAnyImplementationOfIPlayerGenerator()
    {
        var config = new LotteryConfiguration();
        IPlayerGenerator generator1 = new PlayerGenerator(config);
        IPlayerGenerator generator2 = new CustomPlayerGenerator();

        var game1 = new LotteryGame(generator1, new PrizeCalculator(config), config);
        var game2 = new LotteryGame(generator2, new PrizeCalculator(config), config);

        game1.Initialize(5);
        game2.Initialize(5);

        Assert.NotNull(game1.CpuPlayers);
        Assert.NotNull(game2.CpuPlayers);
    }

    [Fact]
    public void LotteryGame_AcceptsAnyImplementationOfIPrizeCalculator()
    {
        var config = new LotteryConfiguration();
        IPrizeCalculator calculator1 = new PrizeCalculator(config);
        IPrizeCalculator calculator2 = new CustomPrizeCalculator();

        var game1 = new LotteryGame(new PlayerGenerator(config), calculator1, config);
        var game2 = new LotteryGame(new PlayerGenerator(config), calculator2, config);

        game1.Initialize(5);
        game2.Initialize(5);

        var results1 = game1.DrawWinners();
        var results2 = game2.DrawWinners();

        Assert.NotNull(results1);
        Assert.NotNull(results2);
    }

    [Fact]
    public void PlayerGenerator_WorksWithDifferentConfigurations()
    {
        var config1 = new LotteryConfiguration { MinTicketCount = 1, MaxTicketCount = 5 };
        var config2 = new LotteryConfiguration { MinTicketCount = 5, MaxTicketCount = 10 };

        var generator1 = new PlayerGenerator(config1);
        var generator2 = new PlayerGenerator(config2);

        var players1 = generator1.GenerateCpuPlayers(1);
        var players2 = generator2.GenerateCpuPlayers(1);

        Assert.All(players1, p => Assert.InRange(p.Tickets.Count, 1, 5));
        Assert.All(players2, p => Assert.InRange(p.Tickets.Count, 5, 10));
    }

    [Fact]
    public void PrizeCalculator_WorksWithDifferentTicketCosts()
    {
        var config1 = new LotteryConfiguration { TicketCost = 1m };
        var config2 = new LotteryConfiguration { TicketCost = 5m };

        var calculator1 = new PrizeCalculator(config1);
        var calculator2 = new PrizeCalculator(config2);

        var tickets = CreateTestTickets(10);

        var grandPrize1 = calculator1.CalculateGrandPrize(tickets);
        var grandPrize2 = calculator2.CalculateGrandPrize(tickets);

        Assert.Equal(5m, grandPrize1);
        Assert.Equal(25m, grandPrize2);
    }

    [Fact]
    public void PrizeCalculator_WorksWithDifferentPrizePercentages()
    {
        var config1 = new LotteryConfiguration { GrandPrizePercentage = 0.5m };
        var config2 = new LotteryConfiguration { GrandPrizePercentage = 0.7m, SecondTierPercentage = 0.2m };

        var calculator1 = new PrizeCalculator(config1);
        var calculator2 = new PrizeCalculator(config2);

        var tickets = CreateTestTickets(10);

        var grandPrize1 = calculator1.CalculateGrandPrize(tickets);
        var grandPrize2 = calculator2.CalculateGrandPrize(tickets);

        Assert.Equal(5m, grandPrize1);
        Assert.Equal(7m, grandPrize2);
    }

    [Fact]
    public void Configuration_CanBeReplacedAtRuntime()
    {
        var config = new LotteryConfiguration
        {
            MinTicketCount = 1,
            MaxTicketCount = 10,
            InitialBalance = 20m,
            TicketCost = 2m,
            GrandPrizePercentage = 0.6m,
            SecondTierPercentage = 0.25m,
            ThirdTierPercentage = 0.05m
        };

        config.Validate();

        var generator = new PlayerGenerator(config);
        var calculator = new PrizeCalculator(config);
        var game = new LotteryGame(generator, calculator, config);

        game.Initialize(5);
        var tickets = game.GetAllTickets();
        var results = game.DrawWinners();
        var gameResults = game.GetGameResults(results);

        var totalRevenue = tickets.Count * 2m;
        var expectedGrandPrize = totalRevenue * 0.6m;

        Assert.Equal(expectedGrandPrize, gameResults.GrandPrizeAmount);
    }

    private List<Ticket> CreateTestTickets(int count)
    {
        var player = new Player("Test", initialBalance: 10m);
        var tickets = new List<Ticket>();
        for (int i = 0; i < count; i++)
        {
            tickets.Add(new Ticket(player));
        }
        return tickets;
    }
}