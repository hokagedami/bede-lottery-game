using LotteryEngine.Configuration;

namespace LotteryEngine.Test.Configuration;

/// <summary>
/// Tests for lottery configuration and its validation
/// </summary>
public class ConfigurationTests
{
    [Fact]
    public void DefaultConfiguration_ShouldBeValid()
    {
        var config = new LotteryConfiguration();

        config.Validate();

        Assert.Equal(1, config.MinTicketCount);
        Assert.Equal(10, config.MaxTicketCount);
        Assert.Equal(10m, config.InitialBalance);
        Assert.Equal(1m, config.TicketCost);
        Assert.Equal(0.5m, config.GrandPrizePercentage);
        Assert.Equal(0.3m, config.SecondTierPercentage);
        Assert.Equal(0.1m, config.ThirdTierPercentage);
    }

    [Fact]
    public void Configuration_WithCustomValues_ShouldWork()
    {
        var config = new LotteryConfiguration
        {
            MinTicketCount = 5,
            MaxTicketCount = 20,
            InitialBalance = 50m,
            TicketCost = 5m,
            GrandPrizePercentage = 0.6m,
            SecondTierPercentage = 0.25m,
            ThirdTierPercentage = 0.05m
        };

        config.Validate();

        Assert.Equal(5, config.MinTicketCount);
        Assert.Equal(20, config.MaxTicketCount);
        Assert.Equal(50m, config.InitialBalance);
        Assert.Equal(5m, config.TicketCost);
    }

    [Fact]
    public void Validate_WithMinTicketCountLessThanOne_ShouldThrowException()
    {
        var config = new LotteryConfiguration { MinTicketCount = 0 };

        var exception = Assert.Throws<ArgumentException>(() => config.Validate());

        Assert.Contains("MinTicketCount must be at least 1", exception.Message);
    }

    [Fact]
    public void Validate_WithMaxTicketCountLessThanMin_ShouldThrowException()
    {
        var config = new LotteryConfiguration
        {
            MinTicketCount = 10,
            MaxTicketCount = 5
        };

        var exception = Assert.Throws<ArgumentException>(() => config.Validate());

        Assert.Contains("MaxTicketCount must be greater than or equal to MinTicketCount", exception.Message);
    }

    [Fact]
    public void Validate_WithZeroTicketCost_ShouldThrowException()
    {
        var config = new LotteryConfiguration { TicketCost = 0 };

        var exception = Assert.Throws<ArgumentException>(() => config.Validate());

        Assert.Contains("TicketCost must be greater than 0", exception.Message);
    }

    [Fact]
    public void Validate_WithNegativeTicketCost_ShouldThrowException()
    {
        var config = new LotteryConfiguration { TicketCost = -1m };

        var exception = Assert.Throws<ArgumentException>(() => config.Validate());

        Assert.Contains("TicketCost must be greater than 0", exception.Message);
    }

    [Fact]
    public void Validate_WithTotalPercentagesExceeding100_ShouldThrowException()
    {
        var config = new LotteryConfiguration
        {
            GrandPrizePercentage = 0.6m,
            SecondTierPercentage = 0.4m,
            ThirdTierPercentage = 0.2m
        };

        var exception = Assert.Throws<ArgumentException>(() => config.Validate());

        Assert.Contains("Total prize percentages cannot exceed 100%", exception.Message);
    }

    [Fact]
    public void Validate_WithExactly100PercentPrizes_ShouldPass()
    {
        var config = new LotteryConfiguration
        {
            GrandPrizePercentage = 0.5m,
            SecondTierPercentage = 0.3m,
            ThirdTierPercentage = 0.2m
        };

        config.Validate();

        Assert.Equal(1.0m, config.GrandPrizePercentage + config.SecondTierPercentage + config.ThirdTierPercentage);
    }

    [Theory]
    [InlineData(0.4, 0.3, 0.1)]
    [InlineData(0.5, 0.2, 0.1)]
    [InlineData(0.6, 0.25, 0.05)]
    public void Validate_WithValidPrizePercentages_ShouldPass(
        decimal grandPrize,
        decimal secondTier,
        decimal thirdTier)
    {
        var config = new LotteryConfiguration
        {
            GrandPrizePercentage = grandPrize,
            SecondTierPercentage = secondTier,
            ThirdTierPercentage = thirdTier
        };

        config.Validate();

        Assert.True(grandPrize + secondTier + thirdTier <= 1.0m);
    }
}
