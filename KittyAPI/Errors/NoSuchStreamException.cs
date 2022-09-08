using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KittyAPI.Errors;

public class NoSuchStreamException : Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public string ErrorMessage => "No such stream exists";
}
