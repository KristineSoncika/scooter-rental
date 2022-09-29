using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests;

public class CalculatorsTests
{
    private readonly Calculations _calculations = new();
    
    public static IEnumerable<object[]> RentalData =>
        new List<object[]>
        {
            new object[] { new DateTime(2022, 09, 01, 0, 0, 0), new DateTime(2022, 09, 02, 0, 0, 0), 0.15m, 20.00 },
            new object[] { new DateTime(2022, 09, 01, 0, 0, 0), new DateTime(2022, 09, 02, 0, 2, 0), 0.15m, 20.30 },
            new object[] { new DateTime(2022, 09, 01, 0, 0, 0), new DateTime(2022, 09, 01, 0, 2, 0), 0.15m, 0.30 },
            new object[] { new DateTime(2022, 09, 01, 12, 0, 0), new DateTime(2022, 09, 01, 16, 30, 0), 0.15m, 20.00 },
            new object[] { new DateTime(2022, 09, 01, 9, 0, 0), new DateTime(2022, 09, 01, 10, 2, 0), 0.15m, 9.30 },
            new object[] { new DateTime(2022, 09, 01, 9, 0, 0), new DateTime(2022, 09, 02, 0, 0, 0), 0.15m, 20.00 },
            new object[] { new DateTime(2022, 09, 01, 9, 0, 0), new DateTime(2022, 09, 02, 02, 0, 0), 0.15m, 38.00 },
            new object[] { new DateTime(2022, 09, 01, 9, 0, 0), new DateTime(2022, 09, 02, 10, 0, 0), 0.15m, 40.00 },
            new object[] { new DateTime(2022, 09, 01, 9, 0, 0), new DateTime(2022, 09, 03, 10, 0, 0), 0.15m, 60.00 },
            new object[] { new DateTime(2022, 09, 01, 23, 0, 0), new DateTime(2022, 09, 03, 10, 0, 0), 0.15m, 49.00 },
            new object[] { new DateTime(2022, 09, 01, 23, 30, 0), new DateTime(2022, 09, 02, 0, 30, 0), 1.00m, 40.00 }
        };
    
    [Theory]
    [MemberData(nameof(RentalData))]
    public void CalculateRentalPrice_ReturnsScooterRentalPrice(DateTime start, DateTime end, decimal pricePerMinute, decimal expected)
    {
        // Act & Assert
       _calculations.CalculateRentalPrice(start, end, pricePerMinute).Should().Be(expected);
    }

    private readonly List<RentedScooter> _rentalsList = new();

    private void RentalsMockData()
    {
        _rentalsList.Add(new RentedScooter("Scooter-1", 0.15m, new DateTime(2022, 01, 01, 00, 05, 00)));

        _rentalsList.Add(new RentedScooter("Scooter-2", 0.15m, new DateTime(2022, 01, 01, 08, 00, 00)));
        
        _rentalsList.Add(new RentedScooter("Scooter-3", 0.15m, new DateTime(2021, 12, 31, 20, 00, 00)));
        
        _rentalsList.Add(new RentedScooter("Scooter-4", 0.15m, new DateTime(2021, 08, 01, 23, 00, 00)));
        _rentalsList[3].RentalEnd = new DateTime(2021, 08, 02, 09, 30, 0);
        
        _rentalsList.Add(new RentedScooter("Scooter-5", 0.15m, new DateTime(2020, 08, 01, 09, 00, 00)));
        _rentalsList[4].RentalEnd = new DateTime(2020, 08, 01, 09, 30, 0);
        
        _rentalsList.Add(new RentedScooter("Scooter-2", 0.15m, new DateTime(2020, 08, 01, 09, 00, 00)));
        _rentalsList[5].RentalEnd = new DateTime(2020, 08, 03, 00, 02, 0);
    }

    private void RentalEndDateMock()
    {
        foreach (var rental in _rentalsList.Where(rental => rental.RentalEnd == null))
        {
            rental.RentalEnd = new DateTime(2022, 01, 01, 08, 05, 0);
        }
    }

   [Fact]
    public void CalculateRentalIncome_OnlyCompletedRentals_ReturnsIncome()
    {
        // Arrange
        RentalsMockData();

        // Act & Assert
        _calculations.CalculateRentalIncome(_rentalsList, false).Should().Be(73.8m);
    }
    
    [Fact]
    public void CalculateRentalIncome_IncludesNotCompletedRentals_ReturnsIncome()
    {
        // Arrange
        RentalsMockData();
        RentalEndDateMock();

        // Act & Assert
        _calculations.CalculateRentalIncome(_rentalsList, true).Should().Be(134.55m);
    }

    [Theory]
    [InlineData(2022, 0.00)]
    [InlineData(2021, 29.00)]
    [InlineData(2020, 44.80)]
    public void CalculateYearlyRentalIncome_OnlyCompletedRentalsInGivenYear_ReturnsYearlyIncome(int year, decimal expected)
    {
        // Arrange
        RentalsMockData();

        // Act & Assert
        _calculations.CalculateYearlyRentalIncome(_rentalsList, false, year).Should().Be(expected);
    }
    
    [Theory]
    [InlineData(2022, 20.75)]
    [InlineData(2021, 69.00)]
    [InlineData(2020, 44.80)]
    public void CalculateYearlyRentalIncome_IncludesNotCompletedRentalsInGivenYear_ReturnsYearlyIncome(int year, decimal expected)
    {
        // Arrange
        RentalsMockData();
        RentalEndDateMock();

        // Act & Assert
        _calculations.CalculateYearlyRentalIncome(_rentalsList, false, year).Should().Be(expected);
    }

    [Fact]
    public void CalculateYearlyRentalIncome_NoRentalsInGivenYear_ThrowsNoRentalsInGivenYearException()
    {
        // Arrange
        RentalsMockData();
        
        // Act
        Action act = () => _calculations.CalculateYearlyRentalIncome(_rentalsList, false, 2018);
        
        // Assert
        act.Should()
            .Throw<NoRentalsInGivenYearException>()
            .WithMessage("There are no rentals in 2018");
    }
}