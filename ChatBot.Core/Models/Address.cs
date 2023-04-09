namespace ChatBot.Core.Models;

public class Address
{
    public long Id { get; set; }
    public string? AddressData { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
}
