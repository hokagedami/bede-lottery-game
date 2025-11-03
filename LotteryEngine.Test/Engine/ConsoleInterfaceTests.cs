using LotteryEngine.Engine;
using LotteryEngine.Entities;
using LotteryEngine.Test.Helpers;

namespace LotteryEngine.Test.Engine;

/// <summary>
/// Tests for the ConsoleInterface class
/// </summary>
public class ConsoleInterfaceTests
{
    [Fact]
    public void DisplayWelcome_ShouldShowWelcomeMessage()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);

        // Act
        consoleInterface.DisplayWelcome();

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("Welcome"));
    }

    [Fact]
    public void DisplayWelcome_ShouldShowStartingBalance()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);

        // Act
        consoleInterface.DisplayWelcome();

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("$10"));
    }

    [Fact]
    public void DisplayWelcome_ShouldShowTicketPrice()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);

        // Act
        consoleInterface.DisplayWelcome();

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("$1"));
    }

    [Theory]
    [InlineData("5", 5)]
    [InlineData("1", 1)]
    [InlineData("10", 10)]
    public void GetTicketCount_WithValidInput_ShouldReturnCount(string input, int expected)
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        mockConsole.QueueInput(input);
        var consoleInterface = new ConsoleInterface(mockConsole);

        // Act
        var result = consoleInterface.GetTicketCount();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetTicketCount_WithInvalidThenValid_ShouldPromptAgain()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        mockConsole.QueueInput("abc");
        mockConsole.QueueInput("5");
        var consoleInterface = new ConsoleInterface(mockConsole);

        // Act
        var result = consoleInterface.GetTicketCount();

        // Assert
        Assert.Equal(5, result);
        Assert.Contains(mockConsole.Output, line => line.Contains("Invalid"));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("11")]
    [InlineData("-1")]
    public void GetTicketCount_WithOutOfRangeValue_ShouldPromptAgain(string invalidInput)
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        mockConsole.QueueInput(invalidInput);
        mockConsole.QueueInput("5");
        var consoleInterface = new ConsoleInterface(mockConsole);

        // Act
        var result = consoleInterface.GetTicketCount();

        // Assert
        Assert.Equal(5, result);
        Assert.Contains(mockConsole.Output, line => line.Contains("Invalid") || line.Contains("between"));
    }

    [Fact]
    public void DisplayCpuPlayers_ShouldShowPlayerInformation()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);
        var cpuPlayers = new List<Player>
        {
            CreatePlayerWithTickets("CPU1", 3),
            CreatePlayerWithTickets("CPU2", 5)
        };

        // Act
        consoleInterface.DisplayCpuPlayers(cpuPlayers);

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("CPU1"));
        Assert.Contains(mockConsole.Output, line => line.Contains("CPU2"));
    }

    [Fact]
    public void DisplayResults_ShouldShowGrandPrizeWinner()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);
        var results = CreateSampleGameResults();

        // Act
        consoleInterface.DisplayResults(results);

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("Grand Prize"));
        Assert.Contains(mockConsole.Output, line => line.Contains(results.GrandPrizeWinner));
    }

    [Fact]
    public void DisplayResults_ShouldShowSecondTierWinners()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);
        var results = CreateSampleGameResults();

        // Act
        consoleInterface.DisplayResults(results);

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("Second Tier") || line.Contains("Tier 2"));
    }

    [Fact]
    public void DisplayResults_ShouldShowThirdTierWinners()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);
        var results = CreateSampleGameResults();

        // Act
        consoleInterface.DisplayResults(results);

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("Third Tier") || line.Contains("Tier 3"));
    }

    [Fact]
    public void DisplayResults_ShouldShowHouseProfit()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);
        var results = CreateSampleGameResults();

        // Act
        consoleInterface.DisplayResults(results);

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("House"));
    }

    [Fact]
    public void DisplayResults_ShouldFormatPrizesAsDollars()
    {
        // Arrange
        var mockConsole = new MockConsoleWrapper();
        var consoleInterface = new ConsoleInterface(mockConsole);
        var results = CreateSampleGameResults();

        // Act
        consoleInterface.DisplayResults(results);

        // Assert
        Assert.Contains(mockConsole.Output, line => line.Contains("$"));
    }

    private static Player CreatePlayerWithTickets(string id, int ticketCount)
    {
        var player = new Player(id, initialBalance: 10m);
        player.PurchaseTickets(ticketCount);
        return player;
    }

    private static GameResults CreateSampleGameResults()
    {
        return new GameResults
        {
            GrandPrizeWinner = "CPU1",
            GrandPrizeAmount = 50m,
            SecondTierWinners = new Dictionary<string, int>
            {
                { "CPU2", 1 },
                { "CPU3", 2 }
            },
            SecondTierPrizePerWinner = 10m,
            ThirdTierWinners = new Dictionary<string, int>
            {
                { "CPU4", 1 },
                { "You", 1 }
            },
            ThirdTierPrizePerWinner = 2m,
            HouseProfit = 5m
        };
    }
}
