using FluentAssertions;

namespace ScooterRental.Tests;

public class ScooterTests
{
    private readonly Scooter _scooter;

    public ScooterTests()
    {
        _scooter = new Scooter("Scooter-1", 0.2m);
    }

    [Fact]
    public void CreateScooter_ValidScooter_CreatesScooter()
    {
        // Acy & Assert
        _scooter.Id.Should().Be("Scooter-1");
        _scooter.PricePerMinute.Should().Be(0.2m);
        _scooter.IsRented.Should().BeFalse();
    }
}