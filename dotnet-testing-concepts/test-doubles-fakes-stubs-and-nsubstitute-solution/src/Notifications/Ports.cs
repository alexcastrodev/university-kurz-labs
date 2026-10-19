namespace Notifications;

// The ports the notification service depends on. Each one is an interface, so a test can replace it
// with a hand-written fake or with a substitute.

public interface ICustomerDirectory
{
    Task<Contact?> FindAsync(Guid customerId, CancellationToken ct);
}

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct);
}

public interface ISmsSender
{
    // True when the gateway accepted the message, false when it refused it (unknown number, quota).
    Task<bool> SendAsync(string phoneNumber, string text, CancellationToken ct);
}

// Remembers which notifications were already delivered, so a shipment event that arrives twice
// notifies the customer once.
public interface INotificationLog
{
    Task<bool> ContainsAsync(string key, CancellationToken ct);

    Task AddAsync(string key, CancellationToken ct);
}
