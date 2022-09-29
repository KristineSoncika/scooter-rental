namespace ScooterRental.Exceptions;

public class NoAvailableScootersFoundException : Exception
{
    public NoAvailableScootersFoundException() : 
        base("There are no scooters currently available.") { }
}