using System.Net;

namespace KittyAPI.Errors;

public class WebsocketError : Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;

    public string ErrorMessage => "No context or connection id found";

    public string ErrorCode => "SignalR.NoContext";
}
