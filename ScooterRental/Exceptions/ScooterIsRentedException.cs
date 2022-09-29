namespace ScooterRental.Exceptions;

public class ScooterIsRentedOutException : Exception
{
    public ScooterIsRentedOutException(string? id) : 
        base($"Rented scooter cannot be removed: {id}.") { }
}