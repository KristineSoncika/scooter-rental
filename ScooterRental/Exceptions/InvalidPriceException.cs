namespace ScooterRental.Exceptions;

public class InvalidPriceException : Exception
{
    public InvalidPriceException(decimal price) : 
        base($"Price must be greater than 0: {price}") { }
}