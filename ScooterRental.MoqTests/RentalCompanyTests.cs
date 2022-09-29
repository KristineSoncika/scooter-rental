using FluentAssertions;
using Moq;
using Moq.AutoMock;
using ScooterRental.Exceptions;
using ScooterRental.Interfaces;

namespace ScooterRental.MoqTests;

public class RentalCompanyTests
{
    private readonly IRentalCompany _rentalCompany;
    private readonly Mock<IScooterService> _scooterServiceMock;
    private readonly Mock<ICalculations> _calculationsMock;
    private readonly List<RentedScooter> _rentalsList;
    private readonly Scooter _defaultScooter;
    private readonly RentedScooter _defaultRentedScooter;

    public RentalCompanyTests()
    {
        var mocker = new AutoMocker();
        _scooterServiceMock = mocker.GetMock<IScooterService>();
        _calculationsMock = mocker.GetMock<ICalculations>();
        _rentalsList = new List<RentedScooter>();
        _defaultScooter = new Scooter("Scooter-1", 0.15m);
        _defaultRentedScooter = new RentedScooter(_defaultScooter.Id, _defaultScooter.PricePerMinute, new DateTime(2022, 09, 09, 21, 0, 0));
        _rentalCompany = new RentalCompany("MockScooters", _scooterServiceMock.Object, _rentalsList, _calculationsMock.Object);
    }

    [Fact]
    public void StartRent_ValidRent_StartsRent()
    {
        // Arrange
        _scooterServiceMock.Setup(service => service.GetScooterById("Scooter-1")).Returns(_defaultScooter); 
        var defaultAvailableScooters = new List<Scooter> { _defaultScooter };
        _scooterServiceMock.Setup(service => service.GetScooters()).Returns(defaultAvailableScooters);
        
        // Act 
        _rentalCompany.StartRent("Scooter-1");
        
        // Assert
        _defaultScooter.IsRented.Should().BeTrue();
        _rentalsList.Should().HaveCount(1);
    }
    
    [Fact]
    public void StartRent_ScooterNotAvailable_ThrowsScooterNotAvailableException()
    {
        // Arrange
        _scooterServiceMock.Setup(service => service.GetScooterById("Scooter-1")).Returns(_defaultScooter);
        var defaultAvailableScooters = new List<Scooter>();
        _scooterServiceMock.Setup(service => service.GetScooters()).Returns(defaultAvailableScooters);
        
        // Act & Assert
        _rentalCompany.Invoking(company => company.StartRent("Scooter-1"))
            .Should().Throw<ScooterNotAvailableException>()
            .WithMessage("This scooter is rented out: Scooter-1");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void StartRent_IdIsNullOrEmpty_ThrowsInvalidIdException(string id)
    {
        // Arrange
        _scooterServiceMock.Setup(service => service.GetScooterById(id)).Throws<InvalidIdException>();
        
        // Act & Assert
        _rentalCompany.Invoking(company => company.StartRent(id))
            .Should().Throw<InvalidIdException>()
            .WithMessage("Id cannot be null or empty.");
    }

    [Fact]
    public void StartRent_ScooterDoesNotExist_ThrowsScooterDoesNotExistException()
    {
        // Arrange
        _scooterServiceMock.Setup(service => service.GetScooterById("Scooter-3")).Throws<ScooterDoesNotExistException>();
        
        // Act & Assert
        _rentalCompany.Invoking(company => company.StartRent("Scooter-3"))
            .Should().Throw<ScooterDoesNotExistException>()
            .WithMessage("Scooter does not exist.");
    }
    
    [Fact]
    public void StartRent_NoAvailableScooters_ThrowsNoAvailableScootersFoundException()
    {
        // Arrange
        _scooterServiceMock.Setup(service => service.GetScooters()).Throws<NoAvailableScootersFoundException>();
        
        // Act & Assert
        _rentalCompany.Invoking(company => company.StartRent("Scooter-1"))
            .Should().Throw<NoAvailableScootersFoundException>()
            .WithMessage("There are no scooters currently available.");
    }

    [Fact]
    public void EndRent_ValidRent_EndsRent()
    {
        // Arrange
        _defaultScooter.IsRented = true;
        _scooterServiceMock.Setup(service => service.GetScooterById("Scooter-1")).Returns(_defaultScooter);
        _rentalsList.Add(_defaultRentedScooter);
        var defaultAvailableScooters = new List<Scooter>();
        _scooterServiceMock.Setup(service => service.GetScooters()).Returns(defaultAvailableScooters);
        
        // Act
        _rentalCompany.EndRent("Scooter-1");
        
        // Assert
        _defaultScooter.IsRented.Should().BeFalse();
        _defaultRentedScooter.RentalEnd.Should().HaveValue();
    }
}