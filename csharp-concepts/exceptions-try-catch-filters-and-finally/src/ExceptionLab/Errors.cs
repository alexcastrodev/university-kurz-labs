namespace ExceptionLab;

public enum Failure { None, NotFound, Io, Transient, Permanent, Unexpected }

public sealed class RemoteException(int statusCode) : Exception($"Remote call failed with {statusCode}")
{
    public int StatusCode { get; } = statusCode;
}

public class OrderRejectedException : Exception
{
    public OrderRejectedException(string message, string orderId, Exception? inner = null)
        // TODO-05: Pass the inner exception to the base constructor and store the orderId.
        : base(message)
    {
    }

    public string OrderId { get; } = "";
}
