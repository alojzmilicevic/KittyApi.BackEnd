using KittyAPI.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace KittyAPI.Controllers;
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        Exception exception = HttpContext.Features.Get<IExceptionHandlerFeature>().Error;

        int statusCode = StatusCodes.Status500InternalServerError;
        string message = "Unknown error occured";
        switch (exception)
        {
            case IServiceException e:
                statusCode = (int)e.StatusCode;
                HttpContext.Items[HttpContextItemKeys.ErrorCode] = e.ErrorCode;
                message = e.ErrorMessage;
                break;
            default:
                break;
        }

        return Problem(statusCode: statusCode, title: message);
    }
}
