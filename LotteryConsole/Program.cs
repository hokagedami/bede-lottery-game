using LotteryEngine.Configuration;
using LotteryEngine.Engine;
using LotteryEngine.Interfaces;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var lotteryConfig = new LotteryConfiguration();
configuration.GetSection("LotteryConfiguration").Bind(lotteryConfig);
lotteryConfig.Validate();

var consoleWrapper = new ConsoleWrapper();
var consoleInterface = new ConsoleInterface(consoleWrapper);

consoleInterface.DisplayWelcome();

var ticketCount = consoleInterface.GetTicketCount();

IPlayerGenerator playerGenerator = new PlayerGenerator(lotteryConfig);
IPrizeCalculator prizeCalculator = new PrizeCalculator(lotteryConfig);

var game = new LotteryGame(playerGenerator, prizeCalculator, lotteryConfig);
game.Initialize(ticketCount);

consoleInterface.DisplayCpuPlayers(game.CpuPlayers);

var drawResults = game.DrawWinners();

var gameResults = game.GetGameResults(drawResults);

consoleInterface.DisplayResults(gameResults);