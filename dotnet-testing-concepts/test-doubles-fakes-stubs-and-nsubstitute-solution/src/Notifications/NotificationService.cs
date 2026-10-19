namespace Notifications;

public sealed class NotificationService(
    ICustomerDirectory directory,
    IEmailSender email,
    ISmsSender sms,
    INotificationLog log)
{
    public async Task<NotificationOutcome> NotifyShippedAsync(Shipment shipment, CancellationToken ct)
    {
        var key = $"shipped:{shipment.OrderId}";
        if (await log.ContainsAsync(key, ct))
        {
            return NotificationOutcome.AlreadySent;
        }

        var contact = await directory.FindAsync(shipment.CustomerId, ct);
        if (contact is null)
        {
            return NotificationOutcome.UnknownCustomer;
        }

        if (contact.OptedOut)
        {
            return NotificationOutcome.OptedOut;
        }

        var outcome = await DeliverAsync(contact, shipment.OrderNumber, ct);
        if (outcome != NotificationOutcome.NoReachableChannel)
        {
            // Recorded only after a delivery succeeded: if the sender throws, nothing is recorded
            // and the next attempt sends the message again.
            await log.AddAsync(key, ct);
        }

        return outcome;
    }

    private async Task<NotificationOutcome> DeliverAsync(Contact contact, string orderNumber, CancellationToken ct)
    {
        var text = $"Your order {orderNumber} has shipped.";

        if (contact.Preferred == Channel.Sms && contact.Phone is not null
            && await sms.SendAsync(contact.Phone, text, ct))
        {
            return NotificationOutcome.SentBySms;
        }

        if (contact.Email is null)
        {
            return NotificationOutcome.NoReachableChannel;
        }

        await email.SendAsync(contact.Email, $"Order {orderNumber} shipped", text, ct);
        return contact.Preferred == Channel.Sms ? NotificationOutcome.FellBackToEmail : NotificationOutcome.SentByEmail;
    }
}
