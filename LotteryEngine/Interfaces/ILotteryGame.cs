

using LotteryEngine.Entities;

namespace LotteryEngine.Interfaces;

/// <summary>
/// Interface for lottery game orchestration
/// </summary>
public interface ILotteryGame
{
    /// <summary>
    /// Gets the human player
    /// </summary>
    Player HumanPlayer { get; }

    /// <summary>
    /// Gets the list of CPU players
    /// </summary>
    List<Player> CpuPlayers { get; }

    /// <summary>
    /// Gets the total number of players in the game
    /// </summary>
    int TotalPlayerCount { get; }

    /// <summary>
    /// Initializes the game with a human player and generates CPU players
    /// </summary>
    /// <param name="humanTicketCount">Number of tickets the human player wants to purchase</param>
    void Initialize(int humanTicketCount);

    /// <summary>
    /// Gets all tickets from all players
    /// </summary>
    /// <returns>List of all tickets in the game</returns>
    List<Ticket> GetAllTickets();

    /// <summary>
    /// Draws winners from all tickets
    /// </summary>
    /// <returns>Drawing results</returns>
    DrawResults DrawWinners();

    /// <summary>
    /// Converts draw results to game results with grouped winners and prize amounts
    /// </summary>
    /// <param name="drawResults">The raw drawing results</param>
    /// <returns>Formatted game results</returns>
    GameResults GetGameResults(DrawResults drawResults);
}
