﻿namespace WebApplication2.Models;

public class Subscription
{
    public int IdSubscription { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EndTime { get; set; }
    public int RenewalPeriod { get; set; }
    public decimal Price { get; set; }
    public ICollection<Payment> Payments { get; set; }
    public ICollection<Discount> Discounts { get; set; }
    public ICollection<Sale> Sales { get; set; }
}