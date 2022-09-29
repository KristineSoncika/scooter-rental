namespace ScooterRental;

public class RentedScooter
{
    public string Id { get; }
    public decimal PricePerMinute { get; }
    public DateTime RentalStart { get; }
    public DateTime? RentalEnd { get; set; }

    public RentedScooter(string id, decimal pricePerMinute, DateTime rentalStart)
    {
        Id = id;
        PricePerMinute = pricePerMinute;
        RentalStart = rentalStart;
    }
}