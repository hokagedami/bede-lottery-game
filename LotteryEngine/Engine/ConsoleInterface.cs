using LotteryEngine.Entities;
using LotteryEngine.Interfaces;

namespace LotteryEngine.Engine;

/// <summary>
/// Handles all console input and output for the lottery game
/// </summary>
public class ConsoleInterface
{
    private readonly IConsoleWrapper _console;

    /// <summary>
    /// Initializes a new instance of the ConsoleInterface class
    /// </summary>
    /// <param name="console">Console wrapper for input/output operations</param>
    public ConsoleInterface(IConsoleWrapper console)
    {
        _console = console;
    }

    /// <summary>
    /// Displays the welcome message and game rules
    /// </summary>
    public void DisplayWelcome()
    {
        _console.WriteLine("Welcome to Bede Lottery!");
        _console.WriteLine("You start with a balance of $10.");
        _console.WriteLine("Each ticket costs $1.");
        _console.WriteLine("");
    }

    /// <summary>
    /// Prompts the user for ticket count and validates input
    /// </summary>
    /// <returns>Number of tickets to purchase</returns>
    public int GetTicketCount()
    {
        while (true)
        {
            _console.Write("How many tickets would you like to buy? (1-10): ");
            var input = _console.ReadLine();

            if (int.TryParse(input, out var count))
            {
                if (count is >= 1 and <= 10)
                {
                    return count;
                }

                _console.WriteLine("Invalid input. Please enter a number between 1 and 10.");
            }
            else
            {
                _console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }

    /// <summary>
    /// Displays information about CPU players
    /// </summary>
    /// <param name="cpuPlayers">List of CPU players</param>
    public void DisplayCpuPlayers(List<Player> cpuPlayers)
    {
        _console.WriteLine("");
        _console.WriteLine("CPU Players:");
        foreach (var player in cpuPlayers)
        {
            _console.WriteLine($"{player.Id}: {player.Tickets.Count} ticket(s) purchased");
        }
        _console.WriteLine("");
    }

    /// <summary>
    /// Displays the game results
    /// </summary>
    /// <param name="results">Game results to display</param>
    public void DisplayResults(GameResults results)
    {
        _console.WriteLine("=== DRAW RESULTS ===");
        _console.WriteLine("");

        _console.WriteLine($"Grand Prize Winner: {results.GrandPrizeWinner} - ${results.GrandPrizeAmount}");
        _console.WriteLine("");

        _console.WriteLine("Second Tier Winners:");
        foreach (var winner in results.SecondTierWinners)
        {
            var ticketText = winner.Value == 1 ? "ticket" : "tickets";
            _console.WriteLine($"  {winner.Key} ({winner.Value} {ticketText}): ${results.SecondTierPrizePerWinner * winner.Value}");
        }
        _console.WriteLine("");

        _console.WriteLine("Third Tier Winners:");
        foreach (var winner in results.ThirdTierWinners)
        {
            var ticketText = winner.Value == 1 ? "ticket" : "tickets";
            _console.WriteLine($"  {winner.Key} ({winner.Value} {ticketText}): ${results.ThirdTierPrizePerWinner * winner.Value}");
        }
        _console.WriteLine("");

        _console.WriteLine($"House Profit: ${results.HouseProfit}");
    }
}
