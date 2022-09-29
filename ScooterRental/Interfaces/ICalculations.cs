namespace ScooterRental.Interfaces;

public interface ICalculations
{
    decimal CalculateRentalPrice(DateTime start, DateTime end, decimal pricePerMinute);
    decimal CalculateRentalIncome(List<RentedScooter> rentals, bool includeNotCompletedRentals);
    decimal CalculateYearlyRentalIncome(List<RentedScooter> rentals, bool includeNotCompletedRentals, int year);
}