using AutoService.Domain;
using FluentAssertions;
using Xunit;

namespace AutoService.Tests;

public class AutoServiceTests
{
    private readonly AutoServiceDataContext _context;

    public AutoServiceTests()
    {
        _context = new AutoServiceDataContext();
        _context.SeedData(15); 
    }

    [Fact]
    public void GetMechanicsByWorkType_ShouldReturnOnlyMechanicsWithMatchingSpecialization()
    {
        // Arrange
        var targetWorkType = _context.WorkTypes.First();
        var expectedSpecialization = targetWorkType.Category;

        // Act
        var result = _context.GetMechanicsByWorkType(targetWorkType);

        // Assert
        result.Should().NotBeEmpty();
        result.All(m => m.Specialization == expectedSpecialization).Should().BeTrue();
    }

    [Fact]
    public void GetClientsByMechanic_ShouldReturnOrderedDistinctClients()
    {
        // Arrange
        var targetMechanic = _context.Mechanics.First();

        // Act
        var result = _context.GetClientsByMechanic(targetMechanic);

        // Assert
        result.Should().BeInAscendingOrder(c => c.FullName);
        result.Should().OnlyHaveUniqueItems(); // Проверка на Distinct
    }

    [Fact]
    public void GetRepeatClientsLastMonth_ShouldReturnOnlyClientsWithMultipleOrders()
    {
        // Act
        var result = _context.GetRepeatClientsLastMonth();

        // Assert
        result.Should().NotBeEmpty();
        result.Values.Should().OnlyContain(count => count > 1); // Все значения > 1
    }

    [Fact]
    public void GetTotalOrderCost_ShouldCalculateCorrectSum()
    {
        // Arrange
        var targetOrder = _context.Orders.First();
        var expectedCost = targetOrder.WorkTypes.Sum(w => w.Cost);

        // Act
        var result = _context.GetTotalOrderCost(targetOrder);

        // Assert
        result.Should().Be(expectedCost);
        result.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetTop5FrequentWorkTypes_ShouldReturnAtMost5Items()
    {
        // Act
        var result = _context.GetTop5FrequentWorkTypes();

        // Assert
        Assert.True(result.Count <= 5);
        result.Should().NotBeEmpty();
    }
}