namespace ScooterRental.Exceptions;

public class NoRentalsInGivenYearException : Exception
{
    public NoRentalsInGivenYearException(int year) : 
        base($"There are no rentals in {year}") { }
}