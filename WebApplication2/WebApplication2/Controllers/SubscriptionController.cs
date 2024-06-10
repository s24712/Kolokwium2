using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Context;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly SubscriptionContext _context;

    public SubscriptionController(SubscriptionContext context)
    {
        _context = context;
    }

    [HttpGet("GetClientSubscriptions")]
    public async Task<IActionResult> GetClientSubscriptions(int idKlienta)
    {
        var client = await _context.Client
            .Include(c => c.Subscriptions)
            .ThenInclude(s => s.Payments)
            .FirstOrDefaultAsync(c => c.IdClient == idKlienta);
        if (client == null)
        {
            return NotFound(new { message = "Client not found" });
        }

        var response = new
        {
            firstName = client.FirstName,
            lastName = client.LastName,
            email = client.Email,
            phone = client.Phone,
            subscriptions = client.Subscriptions.Select(s => new
            {
                IdSubscription = s.IdSubscription,
                Name = s.Name,
                TotalPaidAmount = s.Payments.Sum(p => p.Amount)
            }).ToList()
        };
        return Ok(response);
    }

    [HttpPost("AddPayment")]
    public async Task<IActionResult> AddPayment(int IdClient, int IdSubscription, decimal Payment)
    {
        var client = await _context.Client.FindAsync(IdClient);
        if (client == null) return NotFound(new { message = "Client not found" });

        var subscription = await _context.Subscription
            .Include(s => s.Payments)
            .Include(s => s.Sales)
            .FirstOrDefaultAsync(s => s.IdSubscription == IdSubscription);
        if (subscription == null) return NotFound(new { message = "Subscription not found" });

        if (subscription.EndTime < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Subscription is not active" });
        }

        var sale = subscription.Sales.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
        if (sale == null) return BadRequest(new { message = "No sales record found for this subscription" });

        var nextPaymentDate = sale.CreatedAt.AddMonths(subscription.RenewalPeriod * (subscription.Payments.Count + 1));
        if (subscription.Payments.Any(p => p.Date >= nextPaymentDate))
        {
            return BadRequest(new { message = "Payment already made for this period" });
        }

        var discount = await _context.Discount
            .Where(d => d.IdSubscription == IdSubscription && d.DateFrom <= DateTime.UtcNow &&
                        d.DateTo >= DateTime.UtcNow)
            .OrderByDescending(d => d.Value)
            .FirstOrDefaultAsync();

        var actualPaymentAmount = Payment;
        if (discount != null)
        {
            actualPaymentAmount -= Payment * discount.Value / 100;
        }

        var requiredAmount = subscription.Price;
        if (actualPaymentAmount != requiredAmount)
        {
            return BadRequest(new { message = "Incorrect payment amount" });
        }

        var payment = new Payment
        {
            IdClient = IdClient,
            IdSubscription = IdSubscription,
            Amount = actualPaymentAmount,
            Date = DateTime.UtcNow
        };
        _context.Payment.Add(payment);
        await _context.SaveChangesAsync();

        return Ok(new { Id = payment.IdPayment });
    }
}