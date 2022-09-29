using ScooterRental.Exceptions;

namespace ScooterRental.Validations;

public static class Validator
{
    public static void ValidateId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidIdException();
        }
    }

    public static void ValidateIfIdExists(Scooter scooter)
    {
        if (scooter == null)
        {
            throw new ScooterDoesNotExistException();
        }
    }

    public static void ValidatePricePerMinute(decimal pricePerMinute)
    {
        if (pricePerMinute <= 0)
        {
            throw new InvalidPriceException(pricePerMinute);
        }
    }
    
    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidNameException();
        }
    }
}