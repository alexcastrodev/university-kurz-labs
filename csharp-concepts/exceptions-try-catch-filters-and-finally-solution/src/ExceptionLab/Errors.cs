namespace ExceptionLab;

public enum Failure { None, NotFound, Io, Transient, Permanent, Unexpected }

public sealed class RemoteException(int statusCode) : Exception($"Remote call failed with {statusCode}")
{
    public int StatusCode { get; } = statusCode;
}

public class OrderRejectedException : Exception
{
    public OrderRejectedException(string message, string orderId, Exception? inner = null)
        : base(message, inner)
    {
        OrderId = orderId;
    }

    public string OrderId { get; }
}
