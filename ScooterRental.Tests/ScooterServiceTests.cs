using FluentAssertions;
using ScooterRental.Exceptions;
using ScooterRental.Interfaces;

namespace ScooterRental.Tests;

public class ScooterServiceTests
{
    private readonly IScooterService _scooterService;
    private readonly List<Scooter> _scooterList;

    public ScooterServiceTests()
    {
        _scooterList = new List<Scooter>
        {
            new("Scooter-1", 0.2m)
        };
        _scooterService = new ScooterService(_scooterList);
    }

    [Fact]
    public void AddScooter_ValidScooter_AddsScooter()
    {
        // Act
        _scooterService.AddScooter("Scooter-2", 0.1m);
        
        // Assert
        _scooterList.Should().Contain(scooter => scooter.Id == "Scooter-2");
    }
    
    [Fact]
    public void AddScooter_IdNotUnique_ThrowsIdMustBeUniqueException()
    {
        // Act & Assert
        _scooterService.Invoking(service => service.AddScooter("Scooter-1", 0.1m))
            .Should().Throw<IdMustBeUniqueException>()
            .WithMessage("Scooter with id Scooter-1 already exists.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    public void AddScooter_PriceIsNegativeOrZero_ThrowsInvalidPriceException(decimal price)
    {
        // Act & Assert
        _scooterService.Invoking(service => service.AddScooter("Scooter-2", price))
            .Should().Throw<InvalidPriceException>()
            .WithMessage($"Price must be greater than 0: {price}");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void AddScooter_IdIsNullOrEmpty_ThrowsInvalidIdException(string id)
    {
        // Act & Assert
        _scooterService.Invoking(service => service.AddScooter(id, 0.1m))
            .Should().Throw<InvalidIdException>()
            .WithMessage("Id cannot be null or empty.");
    }

    [Fact]
    public void RemoveScooter_ScooterExists_RemovesScooter()
    {
        // Act
        _scooterService.RemoveScooter("Scooter-1");
        
        // Assert
        _scooterList.Should().NotContain(scooter => scooter.Id == "Scooter-1");
    }
    
    [Fact]
    public void RemoveScooter_ScooterDoesNotExist_ThrowsScooterDoesNotExistException()
    {
        // Act & Assert
        _scooterService.Invoking(service => service.RemoveScooter("Scooter-2"))
            .Should().Throw<ScooterDoesNotExistException>()
            .WithMessage("Scooter does not exist.");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void RemoveScooter_IdIsNullOrEmpty_ThrowsInvalidIdException(string id)
    {
        // Act & Assert
        _scooterService.Invoking(service => service.RemoveScooter(id))
            .Should().Throw<InvalidIdException>()
            .WithMessage("Id cannot be null or empty.");
    }
    
    [Fact]
    public void RemoveScooter_ScooterRentedOut_ThrowsScooterIsRentedOutException()
    {
        // Arrange
        var scooter = _scooterList.Find(scooter => scooter.Id == "Scooter-1");
        scooter.IsRented = true;
        
        // Act & Assert
        _scooterService.Invoking(service => service.RemoveScooter("Scooter-1"))
            .Should().Throw<ScooterIsRentedOutException>()
            .WithMessage("Rented scooter cannot be removed: Scooter-1.");
    }
    
    [Fact]
    public void GetScooters_AtLeastOneScooterAvailable_ReturnsAvailableScootersList()
    {
        // Arrange
        var availableScootersCount = _scooterList.FindAll(scooter => scooter.IsRented == false).Count;

        // Act
        var availableScooters = _scooterService.GetScooters();

        // Assert
        availableScooters.Should().HaveCount(availableScootersCount);
    }
    
    [Fact]
    public void GetScooters_NoScootersAvailable_ThrowsNoAvailableScootersFoundException()
    {
        // Arrange
        var scooter = _scooterList.Find(scooter => scooter.Id == "Scooter-1");
        scooter.IsRented = true;
        
        // Act & Assert
        _scooterService.Invoking(service => service.GetScooters())
            .Should().Throw<NoAvailableScootersFoundException>()
            .WithMessage("There are no scooters currently available.");
    }

    [Fact]
    public void GetScooterById_IdFound_ReturnsScooter()
    {
        // Arrange
        var scooter = _scooterList.Find(scooter => scooter.Id == "Scooter-1");
        
        // Act & Assert
        _scooterService.GetScooterById("Scooter-1").Should().Be(scooter);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GetScooterById_IdIsNullOrEmpty_ThrowsInvalidIdException(string id)
    {
        // Act & Assert
        _scooterService.Invoking(service => service.GetScooterById(id))
            .Should().Throw<InvalidIdException>()
            .WithMessage("Id cannot be null or empty.");
    }
    
    [Fact]
    public void GetScooterById_ScooterDoesNotExist_ThrowsScooterDoesNotExistException()
    {
        // Act & Assert
        _scooterService.Invoking(service => service.GetScooterById("Scooter-2"))
            .Should().Throw<ScooterDoesNotExistException>()
            .WithMessage("Scooter does not exist.");
    }
}