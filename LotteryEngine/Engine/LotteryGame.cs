using LotteryEngine.Configuration;
using LotteryEngine.Entities;
using LotteryEngine.Interfaces;


namespace LotteryEngine.Engine;

/// <summary>
/// Orchestrates the entire lottery game
/// </summary>
public class LotteryGame : ILotteryGame
{
    private readonly IPlayerGenerator _playerGenerator;
    private readonly IPrizeCalculator _prizeCalculator;
    private readonly LotteryConfiguration _config;

    /// <summary>
    /// Gets the human player
    /// </summary>
    public Player HumanPlayer { get; private set; } = null!;

    /// <summary>
    /// Gets the list of CPU players
    /// </summary>
    public List<Player> CpuPlayers { get; private set; } = [];

    /// <summary>
    /// Gets the total number of players in the game
    /// </summary>
    public int TotalPlayerCount => 1 + CpuPlayers.Count;

    /// <summary>
    /// Initializes a new instance of the LotteryGame class with dependency injection
    /// </summary>
    /// <param name="playerGenerator">Player generator service</param>
    /// <param name="prizeCalculator">Prize calculator service</param>
    /// <param name="config">Lottery configuration</param>
    public LotteryGame(IPlayerGenerator playerGenerator, IPrizeCalculator prizeCalculator, LotteryConfiguration config)
    {
        _playerGenerator = playerGenerator ?? throw new ArgumentNullException(nameof(playerGenerator));
        _prizeCalculator = prizeCalculator ?? throw new ArgumentNullException(nameof(prizeCalculator));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Initializes the game with a human player and generates CPU players
    /// </summary>
    /// <param name="humanTicketCount">Number of tickets the human player wants to purchase</param>
    public void Initialize(int humanTicketCount)
    {
        HumanPlayer = new Player("You",
            _config.InitialBalance,
            _config.TicketCost,
            _config.MinTicketCount,
            _config.MaxTicketCount);
        HumanPlayer.PurchaseTickets(humanTicketCount);

        CpuPlayers = _playerGenerator.GenerateCpuPlayers(1);
    }

    /// <summary>
    /// Gets all tickets from all players
    /// </summary>
    /// <returns>List of all tickets in the game</returns>
    public List<Ticket> GetAllTickets()
    {
        var allTickets = new List<Ticket>();
        allTickets.AddRange(HumanPlayer.Tickets);
        foreach (var cpuPlayer in CpuPlayers)
        {
            allTickets.AddRange(cpuPlayer.Tickets);
        }
        return allTickets;
    }

    /// <summary>
    /// Draws winners from all tickets
    /// </summary>
    /// <returns>Drawing results</returns>
    public DrawResults DrawWinners()
    {
        var allTickets = GetAllTickets();
        return _prizeCalculator.DrawWinners(allTickets);
    }

    /// <summary>
    /// Converts draw results to game results with grouped winners and prize amounts
    /// </summary>
    /// <param name="drawResults">The raw drawing results</param>
    /// <returns>Formatted game results</returns>
    public GameResults GetGameResults(DrawResults drawResults)
    {
        var allTickets = GetAllTickets();
        var results = new GameResults
        {
            GrandPrizeAmount = _prizeCalculator.CalculateGrandPrize(allTickets),
            SecondTierPrizePerWinner = _prizeCalculator.CalculateSecondTierPrizePerWinner(
                allTickets, drawResults.SecondTierWinners.Count),
            ThirdTierPrizePerWinner = _prizeCalculator.CalculateThirdTierPrizePerWinner(
                allTickets, drawResults.ThirdTierWinners.Count),
            HouseProfit = _prizeCalculator.CalculateHouseProfit(allTickets)
        };

        // Adjust house profit for rounding
        var distributedPrizes = results.GrandPrizeAmount +
                               (results.SecondTierPrizePerWinner * drawResults.SecondTierWinners.Count) +
                               (results.ThirdTierPrizePerWinner * drawResults.ThirdTierWinners.Count);
        var totalRevenue = allTickets.Count;
        results.HouseProfit = totalRevenue - distributedPrizes;

        results.GrandPrizeWinner = drawResults.GrandPrizeWinners[0].Owner.Id;

        results.SecondTierWinners = GroupWinnersByPlayer(drawResults.SecondTierWinners);
        results.ThirdTierWinners = GroupWinnersByPlayer(drawResults.ThirdTierWinners);

        return results;
    }

    private Dictionary<string, int> GroupWinnersByPlayer(List<Ticket> tickets)
    {
        var grouped = new Dictionary<string, int>();
        foreach (var ticket in tickets)
        {
            var playerId = ticket.Owner.Id;
            grouped.TryAdd(playerId, 0);
            grouped[playerId]++;
        }
        return grouped;
    }
}
