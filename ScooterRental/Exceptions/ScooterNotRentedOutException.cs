namespace ScooterRental.Exceptions;

public class ScooterNotRentedOutException : Exception
{
    public ScooterNotRentedOutException(string id) : 
        base($"This scooter is not rented out: {id}") { }
}