# Bede Lottery Game

A configurable lottery game built using Test-Driven Development (TDD) with .NET 9 and xUnit. This implementation emphasizes clean architecture, dependency injection, and extensibility through configuration.

## Table of Contents
- [Overview](#overview)
- [Game Rules](#game-rules)
- [Key Features](#key-features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Building and Running](#building-and-running)
- [Configuration](#configuration)
- [Testing Strategy](#testing-strategy)
- [Design Philosophy](#design-philosophy)

## Overview

This lottery game allows a human player to purchase tickets and compete against CPU players for prizes. The game distributes prizes across three tiers and calculates house profit. The application features a clean separation of concerns with configurable game parameters and comprehensive test coverage.

## Game Rules

### Default Configuration
- **Starting Balance**: Each player starts with $10
- **Ticket Price**: $1 per ticket
- **Ticket Limits**: Players can purchase 1-10 tickets
- **Total Players**: 1 human player + 9-14 CPU players (total 10-15 players)

### Prize Distribution
- **Grand Prize**: 50% of total revenue - awarded to 1 winning ticket
- **Second Tier**: 30% of total revenue - distributed among 10% of all tickets
- **Third Tier**: 10% of total revenue - distributed among 20% of all tickets
- **House Profit**: Remaining 10% plus any rounding remainders

All these values are configurable through `appsettings.json`.

## Key Features

### Configurability
- **External Configuration**: Game parameters defined in `appsettings.json`
- **Runtime Validation**: Configuration validated on startup
- **Easy Modification**: Change rules without recompiling code

### Dependency Injection
- **Loosely Coupled**: Components communicate through interfaces
- **Testable**: Easy to mock dependencies for unit testing
- **Extensible**: Swap implementations without changing client code

### Clean Architecture
- **Separation of Concerns**: Clear boundaries between layers
- **Entity Layer**: Domain models (Player, Ticket)
- **Engine Layer**: Business logic (PrizeCalculator, PlayerGenerator, LotteryGame)
- **Interface Layer**: Abstractions for testability
- **Configuration Layer**: Centralized configuration management
- **Presentation Layer**: Console interface

## Architecture

```
┌─────────────────────────────────────┐
│      LotteryConsole (Program)       │
│   - Entry point                     │
│   - Configuration loading           │
│   - Dependency setup                │
└────────────┬────────────────────────┘
             │ Uses
             ▼
┌─────────────────────────────────────┐
│         LotteryEngine Library       │
│                                     │
│  ┌───────────────────────────────┐ │
│  │  Interfaces                   │ │
│  │  - ILotteryGame               │ │
│  │  - IPrizeCalculator           │ │
│  │  - IPlayerGenerator           │ │
│  │  - IConsoleWrapper            │ │
│  └───────────────────────────────┘ │
│                                     │
│  ┌───────────────────────────────┐ │
│  │  Engine (Business Logic)      │ │
│  │  - LotteryGame                │ │
│  │  - PrizeCalculator            │ │
│  │  - PlayerGenerator            │ │
│  │  - ConsoleInterface           │ │
│  └───────────────────────────────┘ │
│                                     │
│  ┌───────────────────────────────┐ │
│  │  Entities (Domain Models)     │ │
│  │  - Player                     │ │
│  │  - Ticket                     │ │
│  │  - DrawResults                │ │
│  │  - GameResults                │ │
│  └───────────────────────────────┘ │
│                                     │
│  ┌───────────────────────────────┐ │
│  │  Configuration                │ │
│  │  - LotteryConfiguration       │ │
│  └───────────────────────────────┘ │
└─────────────────────────────────────┘
             ▲
             │ Tested by
             │
┌─────────────────────────────────────┐
│      LotteryEngine.Test             │
│   - 136 unit tests                  │
│   - Integration tests               │
│   - Extensibility tests             │
└─────────────────────────────────────┘
```

## Project Structure

```
BedeLotteryGame/
├── BedeLotteryGame.sln          # Solution file
├── README.md                    # This file
│
├── LotteryEngine/               # Core game logic library
│   ├── Configuration/
│   │   └── LotteryConfiguration.cs    # Centralized configuration class
│   ├── Engine/
│   │   ├── LotteryGame.cs            # Game orchestration
│   │   ├── PrizeCalculator.cs        # Prize calculations and winner selection
│   │   ├── PlayerGenerator.cs        # CPU player generation
│   │   └── ConsoleInterface.cs       # Console I/O operations
│   ├── Entities/
│   │   ├── Player.cs                 # Player domain model
│   │   ├── Ticket.cs                 # Ticket domain model
│   │   ├── DrawResults.cs            # Draw results structure
│   │   └── GameResults.cs            # Game results structure
│   ├── Interfaces/
│   │   ├── ILotteryGame.cs           # Game contract
│   │   ├── IPrizeCalculator.cs       # Prize calculator contract
│   │   ├── IPlayerGenerator.cs       # Player generator contract
│   │   └── IConsoleWrapper.cs        # Console abstraction
│   └── LotteryEngine.csproj
│
├── LotteryEngine.Test/          # xUnit test project (136 tests)
│   ├── Configuration/
│   │   ├── ConfigurationTests.cs              # Configuration validation tests
│   │   └── ConfigurabilityIntegrationTests.cs # End-to-end configuration tests
│   ├── Engine/
│   │   ├── LotteryGameTests.cs         # Game orchestration tests
│   │   ├── PrizeCalculatorTests.cs     # Prize calculation tests
│   │   ├── PlayerGeneratorTests.cs     # CPU generation tests
│   │   └── ConsoleInterfaceTests.cs    # Console interface tests
│   ├── Entities/
│   │   ├── PlayerTests.cs              # Player entity tests
│   │   ├── TicketTests.cs              # Ticket entity tests
│   │   └── PlayerDecouplingTests.cs    # Decoupling verification tests
│   ├── Helpers/
│   │   ├── MockConsoleWrapper.cs       # Mock for testing console I/O
│   │   ├── CustomPlayerGenerator.cs    # Custom generator for testing
│   │   └── CustomPrizeCalculator.cs    # Custom calculator for testing
│   ├── IntegrationTests.cs             # End-to-end game tests
│   ├── ExtensibilityTests.cs           # Extensibility verification tests
│   └── LotteryEngine.Test.csproj
│
└── LotteryConsole/              # Console application
    ├── Program.cs               # Entry point with DI setup
    ├── appsettings.json         # Configuration file
    └── LotteryConsole.csproj
```

## Building and Running

### Prerequisites
- .NET 9.0 SDK or later

### Build and Test

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run all tests (136 tests)
dotnet test

# Clean build outputs
dotnet clean
```

### Run the Console Application

```bash
# Run from the console project directory
dotnet run --project LotteryConsole

# Or run from the solution directory
cd LotteryConsole
dotnet run
```

### Example Output

```
Welcome to the Bede Lottery Game!
You start with a balance of $10.
Each ticket costs $1.

How many tickets would you like to buy? (1-10): 5

CPU Players:
CPU1: 7 ticket(s) purchased
CPU2: 3 ticket(s) purchased
CPU3: 10 ticket(s) purchased
CPU4: 5 ticket(s) purchased
...

=== DRAW RESULTS ===

Grand Prize Winner: CPU3 - $50.00

Second Tier Winners:
  CPU1 (2 tickets): $6.00
  You (1 ticket): $3.00

Third Tier Winners:
  CPU2 (1 ticket): $2.00
  CPU4 (3 tickets): $6.00

House Profit: $10.00
```

## Configuration

### appsettings.json

The game is fully configurable through `appsettings.json`:

```json
{
  "LotteryConfiguration": {
    "MinTicketCount": 1,
    "MaxTicketCount": 10,
    "InitialBalance": 10.0,
    "TicketCost": 1.0,
    "GrandPrizePercentage": 0.5,
    "SecondTierPercentage": 0.3,
    "ThirdTierPercentage": 0.1
  }
}
```

### Configuration Options

| Setting | Type | Description | Default |
|---------|------|-------------|---------|
| `MinTicketCount` | int | Minimum tickets a player can purchase | 1 |
| `MaxTicketCount` | int | Maximum tickets a player can purchase | 10 |
| `InitialBalance` | decimal | Starting balance for each player | 10.0 |
| `TicketCost` | decimal | Cost per lottery ticket | 1.0 |
| `GrandPrizePercentage` | decimal | Percentage of revenue for grand prize (0.0-1.0) | 0.5 |
| `SecondTierPercentage` | decimal | Percentage of revenue for second tier (0.0-1.0) | 0.3 |
| `ThirdTierPercentage` | decimal | Percentage of revenue for third tier (0.0-1.0) | 0.1 |

### Configuration Validation

The configuration is validated on startup:
- `MinTicketCount` must be at least 1
- `MaxTicketCount` must be >= `MinTicketCount`
- `TicketCost` must be greater than 0
- Total prize percentages cannot exceed 100%

Invalid configurations will throw an `ArgumentException` with a descriptive error message.

## Testing Strategy

### Test Coverage: 136 Tests

The solution has comprehensive test coverage with 136 passing tests organized into multiple categories:

#### Unit Tests by Component

1. **Player Tests** (`Entities/PlayerTests.cs`)
   - Constructor validation
   - Ticket purchasing
   - Balance management
   - Validation rules

2. **Ticket Tests** (`Entities/TicketTests.cs`)
   - Ticket creation
   - Owner association
   - ID generation

3. **Prize Calculator Tests** (`Engine/PrizeCalculatorTests.cs`)
   - Grand prize calculation
   - Second tier prize calculation
   - Third tier prize calculation
   - Winner selection
   - House profit calculation
   - Rounding behavior

4. **Player Generator Tests** (`Engine/PlayerGeneratorTests.cs`)
   - CPU player generation
   - Ticket count randomization
   - Player count validation

5. **Lottery Game Tests** (`Engine/LotteryGameTests.cs`)
   - Game initialization
   - Ticket aggregation
   - Winner drawing
   - Results formatting
   - Winner grouping

6. **Console Interface Tests** (`Engine/ConsoleInterfaceTests.cs`)
   - User input validation
   - Output formatting
   - Error handling

7. **Configuration Tests** (`Configuration/ConfigurationTests.cs`)
   - Configuration validation
   - Invalid configuration detection

#### Integration Tests

- **Full Game Flow**: End-to-end tests validating complete game execution
- **Configuration Integration**: Tests verifying configuration affects game behavior
- **Extensibility Tests**: Verifying custom implementations work correctly

### Test Principles

- **Arrange-Act-Assert**: All tests follow AAA pattern
- **Descriptive Names**: Test names describe expected behavior
- **Isolation**: Each test is independent and can run in any order
- **Mock Objects**: `MockConsoleWrapper` for testable console I/O
- **Parameterized Tests**: xUnit `Theory` and `InlineData` for multiple scenarios
- **Edge Cases**: Boundary conditions and error scenarios covered

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests for a specific project
dotnet test LotteryEngine.Test

# Run tests with code coverage (requires coverlet)
dotnet test /p:CollectCoverage=true
```

## Design Philosophy

### Test-Driven Development (TDD)

**Approach**: Write tests before implementation (Red-Green-Refactor cycle)

**Benefits**:
1. **Correctness from the Start**: Tests define expected behavior before coding
2. **Living Documentation**: Tests serve as executable specifications
3. **Confidence in Changes**: Comprehensive test suite enables safe refactoring
4. **Design Feedback**: Writing tests first reveals design issues early
5. **Regression Prevention**: Bugs become tests, preventing reoccurrence

**Evidence**: 136 tests covering all business logic, written before implementation

### Dependency Injection

**Approach**: Constructor injection of interfaces

**Example**:
```csharp
public class LotteryGame : ILotteryGame
{
    private readonly IPlayerGenerator _playerGenerator;
    private readonly IPrizeCalculator _prizeCalculator;
    private readonly LotteryConfiguration _config;

    public LotteryGame(
        IPlayerGenerator playerGenerator,
        IPrizeCalculator prizeCalculator,
        LotteryConfiguration config)
    {
        _playerGenerator = playerGenerator;
        _prizeCalculator = prizeCalculator;
        _config = config;
    }
}
```

**Benefits**:
- **Testability**: Easy to inject mocks for unit testing
- **Flexibility**: Swap implementations without changing client code
- **Extensibility**: Add new implementations (e.g., different prize algorithms)
- **Decoupling**: Components depend on abstractions, not concrete types

### Configuration Over Hard-Coding

**Approach**: Externalize game parameters to `appsettings.json`

**Benefits**:
- **Flexibility**: Change game rules without recompiling
- **Environment-Specific**: Different configs for dev/test/prod
- **Validation**: Configuration validated at startup
- **Transparency**: Non-developers can understand/modify rules

**Evolution**: Started with hard-coded constants, evolved to centralized `LotteryConfiguration` class

### Separation of Concerns

**Layers**:
1. **Entities**: Pure domain models (Player, Ticket)
2. **Engine**: Business logic (PrizeCalculator, PlayerGenerator, LotteryGame)
3. **Interfaces**: Contracts for dependency injection
4. **Configuration**: Centralized configuration management
5. **Presentation**: Console I/O (ConsoleInterface)

**Benefits**:
- **Maintainability**: Changes in one layer don't cascade
- **Testability**: Each layer tested independently
- **Understandability**: Clear responsibility for each component
- **Flexibility**: Easy to add new presentation layers (GUI, Web API)

### Encapsulation and Immutability

**Approach**: Domain models protect their invariants

**Example**:
```csharp
public class Player
{
    public string Id { get; }  // Immutable
    public decimal Balance { get; private set; }  // Controlled access

    public void PurchaseTickets(int ticketCount)
    {
        // Validation ensures business rules are always enforced
        ValidateTicketPurchase(ticketCount);
        Balance -= ticketCount * _ticketCost;
    }
}
```

**Benefits**:
- **Prevent Invalid States**: Business rules enforced at all times
- **Thread Safety**: Immutable properties are inherently thread-safe
- **Clear API**: Public methods reveal what operations are allowed
- **Fail Fast**: Invalid operations throw exceptions immediately

### Clean Code Practices

1. **XML Documentation**: All public classes and methods documented
2. **Descriptive Naming**: Methods and classes have clear, self-documenting names
3. **Small Methods**: Each method has single responsibility
4. **No Magic Numbers**: Constants defined with descriptive names
5. **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion

## Architecture Decisions

### 1. Interface-Based Design

**Decision**: Define interfaces for all major components (`IPrizeCalculator`, `IPlayerGenerator`, `ILotteryGame`)

**Rationale**:
- Enables dependency injection and loose coupling
- Facilitates unit testing with mocks
- Allows custom implementations for extensibility
- Enforces contract between components

**Evidence**: `ExtensibilityTests.cs` demonstrates custom implementations working seamlessly

### 2. Centralized Configuration

**Decision**: Use `LotteryConfiguration` class bound from `appsettings.json`

**Rationale**:
- Single source of truth for all game parameters
- Type-safe configuration with validation
- Easy to modify without code changes
- Supports different configurations for different environments

**Alternative Considered**: Hard-coded constants (original implementation)
**Why Changed**: Hard-coded values require recompilation for changes; configuration is more flexible

### 3. Dependency Injection in Program.cs

**Decision**: Manually construct dependency graph in `Program.cs`

```csharp
IPlayerGenerator playerGenerator = new PlayerGenerator(lotteryConfig);
IPrizeCalculator prizeCalculator = new PrizeCalculator(lotteryConfig);
var game = new LotteryGame(playerGenerator, prizeCalculator, lotteryConfig);
```

**Rationale**:
- Simple console app doesn't need full DI container
- Explicit construction makes dependencies visible
- No additional library dependencies
- Easy to understand for maintainers

**Alternative Considered**: Microsoft.Extensions.DependencyInjection
**Why Not Used**: Adds complexity for minimal benefit in console app

### 4. Static Ticket IDs

**Decision**: Use static counter for globally unique ticket IDs

```csharp
private static int _nextId = 1;
public int Id { get; }
```

**Rationale**:
- Global uniqueness across all tickets
- Simple implementation
- Sequential IDs aid debugging
- Mimics real lottery ticket numbering

**Limitation**: Not thread-safe; acceptable for single-threaded console app

### 5. Floor Rounding for Prize Distribution

**Decision**: Use `Math.Floor` for per-winner prizes; house gets remainders

**Rationale**:
- Fair: All winners in a tier receive equal amounts
- Consistent: Predictable behavior
- House advantage: Like real lotteries, house benefits from rounding
- No loss: Total distributed always equals total revenue

**Alternative Considered**: Ceiling or standard rounding
**Why Not Used**: Could over-distribute (ceiling) or unpredictable with .5 cases (rounding)

### 6. IConsoleWrapper for Testability

**Decision**: Abstract Console I/O behind `IConsoleWrapper` interface

**Rationale**:
- Enables unit testing of console interactions
- Isolates tests from external dependencies
- Fast, deterministic tests with `MockConsoleWrapper`
- Allows alternative UI implementations

**Implementation**: `MockConsoleWrapper` queues inputs and captures outputs

## Summary

This lottery game demonstrates professional software engineering practices:

- **Test-Driven Development**: 136 tests written before implementation
- **Clean Architecture**: Clear separation of concerns and layered design
- **Dependency Injection**: Loose coupling through interfaces
- **Configuration Management**: Externalized, validated configuration
- **SOLID Principles**: Maintainable, extensible, testable code
- **Comprehensive Documentation**: XML docs and detailed README

The solution is production-ready, fully tested, and easily extensible for future requirements.

## License

This project is created as a coding exercise demonstration.
