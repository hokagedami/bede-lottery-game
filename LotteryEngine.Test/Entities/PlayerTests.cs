using LotteryEngine.Entities;

namespace LotteryEngine.Test.Entities;

/// <summary>
/// Tests for the Player class
/// </summary>
public class PlayerTests
{
    [Fact]
    public void Constructor_ShouldCreatePlayerWithIdAndStartingBalance()
    {
        // Arrange & Act
        var player = new Player("Player1", initialBalance: 10m);

        // Assert
        Assert.Equal("Player1", player.Id);
        Assert.Equal(10m, player.Balance);
    }

    [Fact]
    public void Constructor_ShouldCreatePlayerWithEmptyTicketList()
    {
        // Arrange & Act
        var player = new Player("Player1", initialBalance: 10m);

        // Assert
        Assert.Empty(player.Tickets);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void PurchaseTickets_ShouldDeductBalanceCorrectly(int ticketCount)
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);
        var initialBalance = player.Balance;

        // Act
        player.PurchaseTickets(ticketCount);

        // Assert
        Assert.Equal(initialBalance - ticketCount, player.Balance);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void PurchaseTickets_ShouldTrackTicketsCorrectly(int ticketCount)
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);

        // Act
        player.PurchaseTickets(ticketCount);

        // Assert
        Assert.Equal(ticketCount, player.Tickets.Count);
    }

    [Fact]
    public void PurchaseTickets_ShouldNotAllowNegativeBalance()
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);
        player.PurchaseTickets(8); // Balance is now 2

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => player.PurchaseTickets(3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PurchaseTickets_ShouldNotAllowFewerThanOneTicket(int ticketCount)
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => player.PurchaseTickets(ticketCount));
    }

    [Fact]
    public void PurchaseTickets_ShouldNotAllowMoreThanTenTickets()
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => player.PurchaseTickets(11));
    }

    [Fact]
    public void PurchaseTickets_ShouldNotAllowSpendingMoreThanBalance()
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);
        player.PurchaseTickets(5); // Balance is now 5

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => player.PurchaseTickets(6));
    }

    [Fact]
    public void PurchaseTickets_MultiplePurchases_ShouldAccumulateTickets()
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);

        // Act
        player.PurchaseTickets(3);
        player.PurchaseTickets(2);

        // Assert
        Assert.Equal(5, player.Tickets.Count);
        Assert.Equal(5m, player.Balance);
    }
}
