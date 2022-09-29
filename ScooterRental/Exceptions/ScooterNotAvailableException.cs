namespace ScooterRental.Exceptions;

public class ScooterNotAvailableException : Exception
{
    public ScooterNotAvailableException(string id) : 
        base($"This scooter is rented out: {id}") { }
}