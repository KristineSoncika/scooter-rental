using FluentAssertions;
using ScooterRental.Exceptions;
using ScooterRental.Interfaces;

namespace ScooterRental.Tests;

public class RentalCompanyTests
{
    private readonly IRentalCompany _rentalCompany;
    private readonly List<Scooter> _scooterList;
    private readonly List<RentedScooter> _rentalsList;

    public RentalCompanyTests()
    {
        _scooterList = new List<Scooter>
        {
            new("Scooter-1", 0.15m),
            new("Scooter-2", 0.20m)
        };
        _rentalsList = new List<RentedScooter>();
        _rentalCompany = new RentalCompany("BlueScooters", new ScooterService(_scooterList), _rentalsList, new Calculations());
    }

    [Fact]
    public void CreateCompany_ValidCompany_CreatesCompany()
    {
        // Act & Assert
        _rentalCompany.Name.Should().Be("BlueScooters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateCompany_InvalidName_ThrowsInvalidNameException(string name)
    {
        // Act & Assert
        FluentActions.Invoking(() => new RentalCompany(name, new ScooterService(_scooterList), _rentalsList, new Calculations()))
            .Should().Throw<InvalidNameException>()
            .WithMessage("Name cannot be null or empty.");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void StartRent_IdIsNullOrEmpty_ThrowsInvalidIdException(string id)
    {
        // Act & Assert
        _rentalCompany.Invoking(company => company.StartRent(id))
            .Should().Throw<InvalidIdException>()
            .WithMessage("Id cannot be null or empty.");
    }
    
    [Fact]
    public void StartRent_ScooterDoesNotExist_ThrowsScooterDoesNotExistException()
    {
        // Act & Assert
        _rentalCompany.Invoking(company => company.StartRent("Scooter-3"))
            .Should().Throw<ScooterDoesNotExistException>()
            .WithMessage("Scooter does not exist.");
    }
    
    [Fact]
    public void StartRent_NoAvailableScooters_ThrowsNoAvailableScootersFoundException()
    {
        // Arrange
        foreach (var scooter in _scooterList)
        {
            scooter.IsRented = true;
        }
        
        // Act & Assert
        _rentalCompany.Invoking(company => company.StartRent("Scooter-2"))
            .Should().Throw<NoAvailableScootersFoundException>()
            .WithMessage("There are no scooters currently available.");
    }

    [Fact]
    public void StartRent_ScooterNotAvailable_ThrowsScooterNotAvailableException()
    {
        // Arrange
        var scooter = _scooterList.Find(scooter => scooter.Id == "Scooter-1");
        scooter.IsRented = true;

        // Act & Assert
        _rentalCompany.Invoking(company => company.StartRent("Scooter-1"))
            .Should().Throw<ScooterNotAvailableException>()
            .WithMessage("This scooter is rented out: Scooter-1");
    }

    [Fact]
    public void StartRent_ValidRent_StartsRent()
    {
        // Arrange
        var scooter = _scooterList.Find(scooter => scooter.Id == "Scooter-1");

        // Act
        _rentalCompany.StartRent("Scooter-1");
        
        // Assert
        scooter.IsRented.Should().BeTrue();
        _rentalsList.Should().HaveCount(1);
        _rentalsList.Should().Contain(rentedScooter => rentedScooter.Id == "Scooter-1");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void EndRent_IdIsNullOrEmpty_ThrowsInvalidIdException(string id)
    {
        // Act & Assert
        _rentalCompany.Invoking(company => company.EndRent(id))
            .Should().Throw<InvalidIdException>()
            .WithMessage("Id cannot be null or empty.");
    }
    
    [Fact]
    public void EndRent_ScooterDoesNotExist_ThrowsScooterDoesNotExistException()
    {
        // Act & Assert
        _rentalCompany.Invoking(company => company.EndRent("Scooter-3"))
            .Should().Throw<ScooterDoesNotExistException>()
            .WithMessage("Scooter does not exist.");
    }

    [Fact]
    public void EndRent_ScooterNotRentedOut_ThrowsScooterNotRentedOutException()
    {
        // Arrange
        var scooter = _scooterList.Find(scooter => scooter.Id == "Scooter-1");
        scooter.IsRented = false;
        
        // Act & Assert
        _rentalCompany.Invoking(company => company.EndRent("Scooter-1"))
            .Should().Throw<ScooterNotRentedOutException>()
            .WithMessage($"This scooter is not rented out: Scooter-1");
    }

    [Fact]
    public void EndRent_ValidRent_EndsRent()
    {
        // Arrange
        var scooter = _scooterList.FirstOrDefault(scooter => scooter.Id == "Scooter-1");
        scooter.IsRented = true;
        var rentedScooter = new RentedScooter(scooter.Id, scooter.PricePerMinute, DateTime.UtcNow.AddMinutes(-12));
        _rentalsList.Add(rentedScooter);

        // Act
        var result =_rentalCompany.EndRent("Scooter-1");

        // Assert
        scooter.IsRented.Should().BeFalse();
        rentedScooter.RentalEnd.Should().HaveValue();
        result.Should().Be(1.8m);
    }

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

    public static IEnumerable<object[]> ReportDataOnlyCompleteRentals =>
        new List<object[]>
        {
            new object[] { null, false, 73.8},
            new object[] { 2022, false, 0.00},
            new object[] { 2021, false, 29.00},
            new object[] { 2020, false, 44.80}
        };

    [Theory]
    [MemberData(nameof(ReportDataOnlyCompleteRentals))]
    public void CalculateIncome_OnlyCompleteRentals_ReturnsIncome(int? year, bool includeNotCompletedRentals, decimal expected)
    {
        // Arrange
        RentalsMockData();
        
        // Act & Assert
        _rentalCompany.CalculateIncome(year, includeNotCompletedRentals).Should().Be(expected);
    }
    
    public static IEnumerable<object[]> ReportDataIncludesIncompleteRentals =>
        new List<object[]>
        {
            new object[] { null, true, 134.55},
            new object[] { 2022, true, 20.75},
            new object[] { 2021, true, 69.00},
            new object[] { 2020, true, 44.80}
        };

    [Theory]
    [MemberData(nameof(ReportDataIncludesIncompleteRentals))]
    public void CalculateIncome_IncludesIncompleteRentals_ReturnsIncome(int? year, bool includeNotCompletedRentals, decimal expected)
    {
        // Arrange
        RentalsMockData();
        RentalEndDateMock();
        
        // Act & Assert
        _rentalCompany.CalculateIncome(year, includeNotCompletedRentals).Should().Be(expected);
    }

    [Fact]
    public void CalculateIncome_NoRentalsForGivenYear_ThrowsNoRentalsInGivenYearException()
    {
        // Arrange
        RentalsMockData();
        
        // Act & Assert
        _rentalCompany.Invoking(company => company.CalculateIncome(2019, false))
            .Should().Throw<NoRentalsInGivenYearException>()
            .WithMessage("There are no rentals in 2019");
    }
}