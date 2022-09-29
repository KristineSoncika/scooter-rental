using ScooterRental.Exceptions;
using ScooterRental.Interfaces;
using ScooterRental.Validations;

namespace ScooterRental;

public class ScooterService : IScooterService
{
    private readonly List<Scooter> _scooters;
    
    public ScooterService(List<Scooter> scooters)
    {
        _scooters = scooters;
    }

    public void AddScooter(string id, decimal pricePerMinute)
    {
        Validator.ValidateId(id);
        Validator.ValidatePricePerMinute(pricePerMinute);
        
        if (_scooters.Any(scooter => scooter.Id == id))
        {
            throw new IdMustBeUniqueException(id);
        }

        _scooters.Add(new Scooter(id, pricePerMinute));
    }

    public void RemoveScooter(string id)
    {
        var scooter = _scooters.FirstOrDefault(scooter => scooter.Id == id);
        Validator.ValidateId(id);
        Validator.ValidateIfIdExists(scooter);

        if (scooter.IsRented)
        {
            throw new ScooterIsRentedOutException(id);
        }
        
        _scooters.Remove(scooter);
    }

    public IList<Scooter> GetScooters()
    {
        var availableScooters = _scooters.FindAll(scooter => scooter.IsRented == false);
        
        if (availableScooters.Count < 1)
        {
            throw new NoAvailableScootersFoundException();
        }

        return availableScooters.ToList();
    }

    public Scooter GetScooterById(string id)
    {
        var scooter = _scooters.FirstOrDefault(scooter => scooter.Id == id);
        Validator.ValidateId(id);
        Validator.ValidateIfIdExists(scooter);
        
        return scooter;
    }
}