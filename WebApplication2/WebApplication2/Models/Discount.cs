namespace WebApplication2.Models;

public class Discount
{
    public int IdDiscount { get; set; }
    public int IdSubscription { get; set; }
    public Subscription Subscription { get; set; }
    public int Value { get; set; } 
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}