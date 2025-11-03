

using LotteryEngine.Configuration;
using LotteryEngine.Engine;

namespace LotteryEngine.Test;

/// <summary>
/// Tests for the PlayerGenerator class
/// </summary>
public class PlayerGeneratorTests
{
    [Fact]
    public void GenerateCpuPlayers_WithHumanPlayer_ShouldCreateBetween9And14CpuPlayers()
    {
        // Arrange
        var generator = new PlayerGenerator(new LotteryConfiguration());

        // Act
        var cpuPlayers = generator.GenerateCpuPlayers(1); // 1 human player

        // Assert
        Assert.InRange(cpuPlayers.Count, 9, 14);
    }

    [Fact]
    public void GenerateCpuPlayers_TotalPlayers_ShouldBeBetween10And15()
    {
        // Arrange
        var generator = new PlayerGenerator(new LotteryConfiguration());
        var humanPlayerCount = 1;

        // Act
        var cpuPlayers = generator.GenerateCpuPlayers(humanPlayerCount);

        // Assert
        var totalPlayers = humanPlayerCount + cpuPlayers.Count;
        Assert.InRange(totalPlayers, 10, 15);
    }

    [Fact]
    public void GenerateCpuPlayers_ShouldHaveStartingBalance()
    {
        // Arrange
        var generator = new PlayerGenerator(new LotteryConfiguration());

        // Act
        var cpuPlayers = generator.GenerateCpuPlayers(1);

        // Assert
        foreach (var player in cpuPlayers)
        {
            // Balance should be 10 or less (if they've purchased tickets)
            Assert.InRange(player.Balance, 0, 10);
        }
    }

    [Fact]
    public void GenerateCpuPlayers_ShouldPurchaseTickets()
    {
        // Arrange
        var generator = new PlayerGenerator(new LotteryConfiguration());

        // Act
        var cpuPlayers = generator.GenerateCpuPlayers(1);

        // Assert
        foreach (var player in cpuPlayers)
        {
            Assert.InRange(player.Tickets.Count, 1, 10);
        }
    }

    [Fact]
    public void GenerateCpuPlayers_ShouldNotExceedBalance()
    {
        // Arrange
        var generator = new PlayerGenerator(new LotteryConfiguration());

        // Act
        var cpuPlayers = generator.GenerateCpuPlayers(1);

        // Assert
        foreach (var player in cpuPlayers)
        {
            // Each ticket costs $1, so tickets count + balance should equal $10
            Assert.Equal(10, player.Tickets.Count + player.Balance);
        }
    }

    [Fact]
    public void GenerateCpuPlayers_ShouldHaveUniqueIds()
    {
        // Arrange
        var generator = new PlayerGenerator(new LotteryConfiguration());

        // Act
        var cpuPlayers = generator.GenerateCpuPlayers(1);

        // Assert
        var playerIds = cpuPlayers.Select(p => p.Id).ToList();
        var uniqueIds = playerIds.Distinct().ToList();
        Assert.Equal(playerIds.Count, uniqueIds.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GenerateCpuPlayers_WithDifferentHumanCounts_ShouldAdjustCpuCount(int humanPlayerCount)
    {
        // Arrange
        var generator = new PlayerGenerator(new LotteryConfiguration());

        // Act
        var cpuPlayers = generator.GenerateCpuPlayers(humanPlayerCount);

        // Assert
        var totalPlayers = humanPlayerCount + cpuPlayers.Count;
        Assert.InRange(totalPlayers, 10, 15);
    }

    [Fact]
    public void GenerateCpuPlayers_MultipleCalls_ShouldProduceDifferentResults()
    {
        // Arrange
        var generator = new PlayerGenerator(new LotteryConfiguration());

        // Act
        var cpuPlayers1 = generator.GenerateCpuPlayers(1);
        var cpuPlayers2 = generator.GenerateCpuPlayers(1);

        // Assert
        // The counts could be the same, but ticket purchases should likely differ
        var totalTickets1 = cpuPlayers1.Sum(p => p.Tickets.Count);
        var totalTickets2 = cpuPlayers2.Sum(p => p.Tickets.Count);

        // This could theoretically fail, but probability is very low
        Assert.NotEqual(totalTickets1, totalTickets2);
    }
}
