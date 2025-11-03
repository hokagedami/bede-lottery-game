using LotteryEngine.Entities;

namespace LotteryEngine.Test;

/// <summary>
/// Tests demonstrating Player decoupling from configuration
/// </summary>
public class PlayerDecouplingTests
{
    [Fact]
    public void Player_CanBeCreated_WithoutConfiguration()
    {
        var player = new Player("TestPlayer", initialBalance: 100m);

        Assert.Equal("TestPlayer", player.Id);
        Assert.Equal(100m, player.Balance);
        Assert.Empty(player.Tickets);
    }

    [Fact]
    public void Player_WithCustomParameters_WorksCorrectly()
    {
        var player = new Player("Player1", initialBalance: 50m, ticketCost: 5m, minTicketCount: 2, maxTicketCount: 8);

        player.PurchaseTickets(3);

        Assert.Equal(3, player.Tickets.Count);
        Assert.Equal(35m, player.Balance);
    }

    [Fact]
    public void Player_WithDefaultParameters_UsesStandardRules()
    {
        var player = new Player("Player1", initialBalance: 10m);

        player.PurchaseTickets(5);

        Assert.Equal(5, player.Tickets.Count);
        Assert.Equal(5m, player.Balance);
    }

    [Fact]
    public void Player_WithExplicitParameters_WorksLikeConfiguration()
    {
        var player = new Player("ConfigPlayer",
            initialBalance: 20m,
            ticketCost: 2m,
            minTicketCount: 1,
            maxTicketCount: 10);

        Assert.Equal("ConfigPlayer", player.Id);
        Assert.Equal(20m, player.Balance);

        player.PurchaseTickets(5);
        Assert.Equal(5, player.Tickets.Count);
        Assert.Equal(10m, player.Balance);
    }

    [Fact]
    public void Player_EnforcesCustomMinTicketCount()
    {
        var player = new Player("Player1", initialBalance: 50m, ticketCost: 1m, minTicketCount: 5, maxTicketCount: 20);

        var exception = Assert.Throws<ArgumentException>(() => player.PurchaseTickets(3));

        Assert.Contains("must be between 5 and 20", exception.Message);
    }

    [Fact]
    public void Player_EnforcesCustomMaxTicketCount()
    {
        var player = new Player("Player1", initialBalance: 50m, ticketCost: 1m, minTicketCount: 1, maxTicketCount: 5);

        var exception = Assert.Throws<ArgumentException>(() => player.PurchaseTickets(10));

        Assert.Contains("must be between 1 and 5", exception.Message);
    }

    [Fact]
    public void Player_EnforcesCustomTicketCost()
    {
        var player = new Player("Player1", initialBalance: 20m, ticketCost: 3m);

        player.PurchaseTickets(5);

        Assert.Equal(5m, player.Balance);
    }

    [Fact]
    public void TwoPlayers_WithDifferentRules_CanCoexist()
    {
        var player1 = new Player("Casual", initialBalance: 10m, ticketCost: 1m, minTicketCount: 1, maxTicketCount: 10);
        var player2 = new Player("HighRoller", initialBalance: 1000m, ticketCost: 100m, minTicketCount: 1, maxTicketCount: 50);

        player1.PurchaseTickets(5);
        player2.PurchaseTickets(10);

        Assert.Equal(5m, player1.Balance);
        Assert.Equal(0m, player2.Balance);
        Assert.Equal(5, player1.Tickets.Count);
        Assert.Equal(10, player2.Tickets.Count);
    }
}
