using ScooterRental.Exceptions;
using ScooterRental.Interfaces;

namespace ScooterRental;

public class Calculations : ICalculations
{
    private const decimal MaxPricePerDay = 20.0m;

    private static decimal CalculatePriceForPeriodWithinOneDay(DateTime start, DateTime end, decimal pricePerMinute)
    {
        var price = (decimal)(end - start).TotalMinutes * pricePerMinute;
        
        return price > MaxPricePerDay ? MaxPricePerDay : Math.Round(price, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal CalculatePriceForPeriodMoreThanOneDay(DateTime start, DateTime end,
        decimal pricePerMinute)
    {
        decimal rentalPrice = 0;
        const int minutesInDay = 1440;
        decimal rentalDays = (end.Date - start.Date).Days - 1;
        
        var firstDayPrice = (minutesInDay - (decimal)start.TimeOfDay.TotalMinutes) * pricePerMinute;
        rentalPrice += firstDayPrice > MaxPricePerDay ? MaxPricePerDay : firstDayPrice;
        
        var lastDayPrice = (decimal)end.TimeOfDay.TotalMinutes * pricePerMinute;
        rentalPrice += lastDayPrice > MaxPricePerDay ? MaxPricePerDay : lastDayPrice;
        
        var restOfDaysPrice = MaxPricePerDay * rentalDays;
        rentalPrice += restOfDaysPrice;
        
        return Math.Round(rentalPrice, 2, MidpointRounding.AwayFromZero);
    }
        
    public decimal CalculateRentalPrice(DateTime start, DateTime end, decimal pricePerMinute)
    {
        return start.Day == end.Day ? 
            CalculatePriceForPeriodWithinOneDay(start, end, pricePerMinute) : 
            CalculatePriceForPeriodMoreThanOneDay(start, end, pricePerMinute);
    }

    public decimal CalculateRentalIncome(List<RentedScooter> rentals, bool includeNotCompletedRentals)
    {
        decimal sum = 0;
        
        if (includeNotCompletedRentals == false)
        {
            sum = rentals
                .Where(rental => rental.RentalEnd != null)
                .Sum(rental => CalculateRentalPrice(rental.RentalStart, (DateTime)rental.RentalEnd, rental.PricePerMinute));

            return Math.Round(sum, 2, MidpointRounding.AwayFromZero);
        }
        
        foreach (var rental in rentals)
        {
            if (rental.RentalEnd == null)
            {
                rental.RentalEnd = DateTime.Now;
            }

            sum += CalculateRentalPrice(rental.RentalStart, (DateTime)rental.RentalEnd, rental.PricePerMinute);
        }

        return Math.Round(sum, 2, MidpointRounding.AwayFromZero);
    }

    public decimal CalculateYearlyRentalIncome(List<RentedScooter> rentals, bool includeNotCompletedRentals, int year)
    {
        var rentalsInGivenYear = rentals.FindAll(rental => rental.RentalStart.Year == year);
        
        if (rentalsInGivenYear.Count < 1)
        {
            throw new NoRentalsInGivenYearException(year);
        }
        
        return CalculateRentalIncome(rentalsInGivenYear, includeNotCompletedRentals);
    }
}