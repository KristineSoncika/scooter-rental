namespace ScooterRental.Exceptions;

public class ScooterDoesNotExistException : Exception
{
    public ScooterDoesNotExistException() : 
        base($"Scooter does not exist.") { }
}