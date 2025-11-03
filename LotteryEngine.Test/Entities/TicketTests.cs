
using LotteryEngine.Entities;

namespace LotteryEngine.Test.Entities;

/// <summary>
/// Tests for the Ticket class
/// </summary>
public class TicketTests
{
    [Fact]
    public void Constructor_ShouldCreateTicketWithOwner()
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);

        // Act
        var ticket = new Ticket(player);

        // Assert
        Assert.Equal(player, ticket.Owner);
    }

    [Fact]
    public void Constructor_ShouldAssignUniqueId()
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);

        // Act
        var ticket1 = new Ticket(player);
        var ticket2 = new Ticket(player);

        // Assert
        Assert.NotEqual(ticket1.Id, ticket2.Id);
    }

    [Fact]
    public void Constructor_MultiplePlayers_ShouldHaveUniqueIds()
    {
        // Arrange
        var player1 = new Player("Player1", initialBalance: 10m);
        var player2 = new Player("Player2", initialBalance: 10m);

        // Act
        var ticket1 = new Ticket(player1);
        var ticket2 = new Ticket(player2);
        var ticket3 = new Ticket(player1);

        // Assert
        Assert.NotEqual(ticket1.Id, ticket2.Id);
        Assert.NotEqual(ticket1.Id, ticket3.Id);
        Assert.NotEqual(ticket2.Id, ticket3.Id);
    }

    [Fact]
    public void Owner_ShouldBelongToExactlyOnePlayer()
    {
        // Arrange
        var player1 = new Player("Player1", initialBalance: 10m);
        var player2 = new Player("Player2", initialBalance: 10m);

        // Act
        var ticket = new Ticket(player1);

        // Assert
        Assert.Equal(player1, ticket.Owner);
        Assert.NotEqual(player2, ticket.Owner);
    }

    [Fact]
    public void Id_ShouldBePositive()
    {
        // Arrange
        var player = new Player("Player1", initialBalance: 10m);

        // Act
        var ticket = new Ticket(player);

        // Assert
        Assert.True(ticket.Id > 0);
    }
}
