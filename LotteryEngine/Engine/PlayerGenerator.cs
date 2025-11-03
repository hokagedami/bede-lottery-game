using LotteryEngine.Configuration;
using LotteryEngine.Entities;
using LotteryEngine.Interfaces;

namespace LotteryEngine.Engine;

/// <summary>
/// Generates CPU players for the lottery game
/// </summary>
public class PlayerGenerator : IPlayerGenerator
{
    private const int MinTotalPlayers = 10;
    private const int MaxTotalPlayers = 15;

    private readonly LotteryConfiguration _config;
    private readonly Random _random = new();

    public PlayerGenerator(LotteryConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Generates CPU players to reach the target total player count
    /// </summary>
    /// <param name="humanPlayerCount">The number of human players already in the game</param>
    /// <returns>List of CPU players with tickets purchased</returns>
    public List<Player> GenerateCpuPlayers(int humanPlayerCount)
    {
        var totalPlayers = _random.Next(MinTotalPlayers, MaxTotalPlayers + 1);
        var cpuPlayerCount = totalPlayers - humanPlayerCount;

        var cpuPlayers = new List<Player>();

        for (var i = 0; i < cpuPlayerCount; i++)
        {
            var player = new Player($"CPU{i + 1}",
                _config.InitialBalance,
                _config.TicketCost,
                _config.MinTicketCount,
                _config.MaxTicketCount);

            var ticketsToBuy = _random.Next(_config.MinTicketCount, _config.MaxTicketCount + 1);
            player.PurchaseTickets(ticketsToBuy);

            cpuPlayers.Add(player);
        }

        return cpuPlayers;
    }
}
