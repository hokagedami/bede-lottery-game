using LotteryEngine.Entities;
using LotteryEngine.Interfaces;

namespace LotteryEngine.Test.Helpers;

/// <summary>
/// Custom player generator for testing extensibility
/// </summary>
public class CustomPlayerGenerator : IPlayerGenerator
{
    public List<Player> GenerateCpuPlayers(int humanPlayerCount)
    {
        var players = new List<Player>
        {
            new Player("CUSTOM1", initialBalance: 10m),
            new Player("CUSTOM2", initialBalance: 10m),
            new Player("CUSTOM3", initialBalance: 10m)
        };

        foreach (var player in players)
        {
            player.PurchaseTickets(5);
        }

        return players;
    }
}