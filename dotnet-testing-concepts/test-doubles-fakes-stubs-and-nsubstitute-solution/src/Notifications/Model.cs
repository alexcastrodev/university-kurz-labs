namespace Notifications;

public enum Channel
{
    Email,
    Sms,
}

public sealed record Contact(Guid CustomerId, string? Email, string? Phone, Channel Preferred, bool OptedOut = false);

public sealed record Shipment(Guid OrderId, Guid CustomerId, string OrderNumber);

public enum NotificationOutcome
{
    SentByEmail,
    SentBySms,
    FellBackToEmail,
    AlreadySent,
    UnknownCustomer,
    OptedOut,
    NoReachableChannel,
}

public sealed class EmailDeliveryException(string message) : Exception(message);
