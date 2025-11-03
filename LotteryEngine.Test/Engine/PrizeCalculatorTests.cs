using LotteryEngine.Configuration;
using LotteryEngine.Engine;
using LotteryEngine.Entities;

namespace LotteryEngine.Test.Engine;

/// <summary>
/// Tests for the PrizeCalculator class
/// </summary>
public class PrizeCalculatorTests
{
    [Theory]
    [InlineData(10, 10)]
    [InlineData(50, 50)]
    [InlineData(100, 100)]
    public void CalculateTotalRevenue_ShouldBeEqualToTicketCount(int ticketCount, decimal expectedRevenue)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var revenue = calculator.CalculateTotalRevenue(tickets);

        // Assert
        Assert.Equal(expectedRevenue, revenue);
    }

    [Theory]
    [InlineData(10, 5)]    // 50% of 10
    [InlineData(50, 25)]   // 50% of 50
    [InlineData(100, 50)]  // 50% of 100
    public void CalculateGrandPrize_ShouldBeFiftyPercentOfRevenue(int ticketCount, decimal expectedPrize)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var grandPrize = calculator.CalculateGrandPrize(tickets);

        // Assert
        Assert.Equal(expectedPrize, grandPrize);
    }

    [Theory]
    [InlineData(10, 3)]    // 30% of 10
    [InlineData(50, 15)]   // 30% of 50
    [InlineData(100, 30)]  // 30% of 100
    public void CalculateSecondTierPool_ShouldBeThirtyPercentOfRevenue(int ticketCount, decimal expectedPool)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var secondTierPool = calculator.CalculateSecondTierPool(tickets);

        // Assert
        Assert.Equal(expectedPool, secondTierPool);
    }

    [Theory]
    [InlineData(10, 1)]    // 10% of 10
    [InlineData(50, 5)]    // 10% of 50
    [InlineData(100, 10)]  // 10% of 100
    public void CalculateThirdTierPool_ShouldBeTenPercentOfRevenue(int ticketCount, decimal expectedPool)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var thirdTierPool = calculator.CalculateThirdTierPool(tickets);

        // Assert
        Assert.Equal(expectedPool, thirdTierPool);
    }

    [Theory]
    [InlineData(10, 1)]    // 10 - 5 - 3 - 1 = 1
    [InlineData(50, 5)]    // 50 - 25 - 15 - 5 = 5
    [InlineData(100, 10)]  // 100 - 50 - 30 - 10 = 10
    public void CalculateHouseProfit_ShouldBeRemainingRevenue(int ticketCount, decimal expectedProfit)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var houseProfit = calculator.CalculateHouseProfit(tickets);

        // Assert
        Assert.Equal(expectedProfit, houseProfit);
    }

    [Fact]
    public void DrawWinners_GrandPrize_ShouldSelectExactlyOneTicket()
    {
        // Arrange
        var tickets = CreateTickets(50);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var results = calculator.DrawWinners(tickets);

        // Assert
        Assert.Single(results.GrandPrizeWinners);
    }

    [Theory]
    [InlineData(10, 1)]    // 10% of 10 = 1
    [InlineData(50, 5)]    // 10% of 50 = 5
    [InlineData(100, 10)]  // 10% of 100 = 10
    public void DrawWinners_SecondTier_ShouldSelectTenPercentOfTickets(int ticketCount, int expectedWinners)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var results = calculator.DrawWinners(tickets);

        // Assert
        Assert.Equal(expectedWinners, results.SecondTierWinners.Count);
    }

    [Theory]
    [InlineData(10, 2)]    // 20% of 10 = 2
    [InlineData(50, 10)]   // 20% of 50 = 10
    [InlineData(100, 20)]  // 20% of 100 = 20
    public void DrawWinners_ThirdTier_ShouldSelectTwentyPercentOfTickets(int ticketCount, int expectedWinners)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var results = calculator.DrawWinners(tickets);

        // Assert
        Assert.Equal(expectedWinners, results.ThirdTierWinners.Count);
    }

    [Fact]
    public void DrawWinners_ShouldNotSelectSameTicketMultipleTimes()
    {
        // Arrange
        var tickets = CreateTickets(50);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var results = calculator.DrawWinners(tickets);

        // Assert
        var allWinners = new List<Ticket>();
        allWinners.AddRange(results.GrandPrizeWinners);
        allWinners.AddRange(results.SecondTierWinners);
        allWinners.AddRange(results.ThirdTierWinners);

        var uniqueWinners = allWinners.Distinct().ToList();
        Assert.Equal(allWinners.Count, uniqueWinners.Count);
    }

    [Theory]
    [InlineData(73)]  // Edge case with prime number
    [InlineData(47)]  // Another edge case
    public void DrawWinners_WithRounding_ShouldHandleCorrectly(int ticketCount)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var results = calculator.DrawWinners(tickets);

        // Assert
        // Verify that all winners are unique
        var allWinners = new List<Ticket>();
        allWinners.AddRange(results.GrandPrizeWinners);
        allWinners.AddRange(results.SecondTierWinners);
        allWinners.AddRange(results.ThirdTierWinners);

        Assert.Equal(allWinners.Count, allWinners.Distinct().Count());

        // Verify grand prize is 1
        Assert.Single(results.GrandPrizeWinners);

        // Verify second tier is approximately 10%
        var expectedSecondTier = (int)Math.Floor(ticketCount * 0.10);
        Assert.Equal(expectedSecondTier, results.SecondTierWinners.Count);

        // Verify third tier is approximately 20%
        var expectedThirdTier = (int)Math.Floor(ticketCount * 0.20);
        Assert.Equal(expectedThirdTier, results.ThirdTierWinners.Count);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(73)]
    [InlineData(100)]
    public void PrizeDistribution_WithRounding_ShouldDivideEvenly(int ticketCount)
    {
        // Arrange
        var tickets = CreateTickets(ticketCount);
        var calculator = new PrizeCalculator(new LotteryConfiguration());
        var results = calculator.DrawWinners(tickets);

        // Act
        var secondTierPerWinner = calculator.CalculateSecondTierPrizePerWinner(tickets, results.SecondTierWinners.Count);
        var thirdTierPerWinner = calculator.CalculateThirdTierPrizePerWinner(tickets, results.ThirdTierWinners.Count);

        // Assert
        // Verify prizes are non-negative
        Assert.True(secondTierPerWinner >= 0);
        Assert.True(thirdTierPerWinner >= 0);

        // Verify the total distribution doesn't exceed revenue
        var totalPrizes = calculator.CalculateGrandPrize(tickets) +
                         (secondTierPerWinner * results.SecondTierWinners.Count) +
                         (thirdTierPerWinner * results.ThirdTierWinners.Count);
        Assert.True(totalPrizes <= ticketCount);
    }

    [Fact]
    public void DrawWinners_ShouldReturnDifferentResultsOnMultipleCalls()
    {
        // Arrange
        var tickets = CreateTickets(50);
        var calculator = new PrizeCalculator(new LotteryConfiguration());

        // Act
        var results1 = calculator.DrawWinners(tickets);
        var results2 = calculator.DrawWinners(tickets);

        // Assert
        // The draws should be random, so they should likely be different
        // (This could theoretically fail, but probability is extremely low)
        Assert.NotEqual(results1.GrandPrizeWinners[0].Id, results2.GrandPrizeWinners[0].Id);
    }

    private static List<Ticket> CreateTickets(int count)
    {
        var tickets = new List<Ticket>();
        for (var i = 0; i < count; i++)
        {
            var player = new Player($"Player{i % 10}", initialBalance: 10m);
            var ticket = new Ticket(player);
            tickets.Add(ticket);
        }
        return tickets;
    }
}
