using System.Net;

namespace KittyAPI.Errors;

public interface IServiceException
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage { get; }
    public string ErrorCode { get; }
}
