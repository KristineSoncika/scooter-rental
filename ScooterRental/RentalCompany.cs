using ScooterRental.Exceptions;
using ScooterRental.Interfaces;
using ScooterRental.Validations;

namespace ScooterRental;

public class RentalCompany : IRentalCompany
{
    private readonly IScooterService _scooterService;
    private readonly List<RentedScooter> _rentals;
    private readonly ICalculations _calculations;

    public RentalCompany(string name, IScooterService scooterService, List<RentedScooter> rentedScooters, ICalculations calculations)
    {
        Validator.ValidateName(name);
        
        Name = name;
        _scooterService = scooterService;
        _rentals = rentedScooters;
        _calculations = calculations;
    }
    
    public string Name { get; }
    
    public void StartRent(string id)
    {
        var scooter = _scooterService.GetScooterById(id);
        var availableScooters = _scooterService.GetScooters();

        if (!availableScooters.Contains(scooter))
        {
            throw new ScooterNotAvailableException(id);
        }
        
        scooter.IsRented = true;
        _rentals.Add(new RentedScooter(scooter.Id, scooter.PricePerMinute, DateTime.UtcNow));
    }

    public decimal EndRent(string id)
    {
        var scooter = _scooterService.GetScooterById(id);
        var availableScooters = _scooterService.GetScooters();
        
        if (availableScooters.Contains(scooter))
        {
            throw new ScooterNotRentedOutException(id);
        }

        scooter.IsRented = false;
        
        var rentedScooter = _rentals.FirstOrDefault(scooter => scooter.Id == id && !scooter.RentalEnd.HasValue);
        rentedScooter.RentalEnd = DateTime.UtcNow;

        return _calculations.CalculateRentalPrice(rentedScooter.RentalStart, (DateTime)rentedScooter.RentalEnd, rentedScooter.PricePerMinute);
    }

    public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
    {
        return year == null ?
            _calculations.CalculateRentalIncome(_rentals, includeNotCompletedRentals) : 
            _calculations.CalculateYearlyRentalIncome(_rentals, includeNotCompletedRentals, (int)year);
    }
}