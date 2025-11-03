

using LotteryEngine.Entities;

namespace LotteryEngine.Interfaces;

/// <summary>
/// Interface for generating lottery players
/// </summary>
public interface IPlayerGenerator
{
    /// <summary>
    /// Generates CPU players for the lottery game
    /// </summary>
    /// <param name="humanTicketCount">Number of tickets the human player purchased</param>
    /// <returns>List of generated CPU players</returns>
    List<Player> GenerateCpuPlayers(int humanTicketCount);
}
