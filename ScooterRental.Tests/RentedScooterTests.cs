using FluentAssertions;

namespace ScooterRental.Tests;

public class RentedScooterTests
{
    private readonly RentedScooter _rentedScooter;

    public RentedScooterTests()
    {
        _rentedScooter = new RentedScooter("Scooter-1", 0.2m, new DateTime(2022, 09, 01));
    }

    [Fact]
    public void CreateRentedScooter_ValidScooter_CreatesScooter()
    {
        // Act & Assert
        _rentedScooter.Id.Should().Be("Scooter-1");
        _rentedScooter.PricePerMinute.Should().Be(0.2m);
        _rentedScooter.RentalStart.Should().Be(new DateTime(2022, 09, 01));
        _rentedScooter.RentalEnd.Should().Be(null);
    }
}