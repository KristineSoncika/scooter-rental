namespace ScooterRental.Exceptions;

public class IdMustBeUniqueException : Exception
{
    public IdMustBeUniqueException(string id) : 
        base($"Scooter with id {id} already exists.") { }
}