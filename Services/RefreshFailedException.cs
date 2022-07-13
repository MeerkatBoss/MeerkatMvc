using System.Runtime.Serialization;

namespace MeerkatMvc.Services;

public class RefreshFailedException : Exception
{
    public RefreshFailedException()
    {
    }

    public RefreshFailedException(string? message) : base(message)
    {
    }

    public RefreshFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected RefreshFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
