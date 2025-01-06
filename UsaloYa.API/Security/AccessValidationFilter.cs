using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UsaloYa.API.Models;
namespace UsaloYa.API.Security
{


    public class AccessValidationFilter : IActionFilter
    {
        private readonly DBContext _dbContext;

        public AccessValidationFilter(DBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId) || string.IsNullOrEmpty(deviceId))
            {
                context.Result = new UnauthorizedObjectResult("*Dispositivo no reconocido."); 
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("RequestorId", out var userIdStr) || string.IsNullOrEmpty(userIdStr))
            {
                context.Result = new UnauthorizedObjectResult("*Usuario no reconocido."); 
                return;
            }

            if (!int.TryParse(userIdStr, out var userId) || userId <= 0)
            {
                context.Result = new UnauthorizedObjectResult("*Usuario no reconocido.");
                return;
            }

            bool skipUserValidation = false;
            if (context.HttpContext.Request.Path.StartsWithSegments(new PathString("/api/User/GetUser")) && context.HttpContext.Request.QueryString.HasValue)
            {
                var queryValue = context.HttpContext.Request.QueryString.Value;
                if (queryValue != null && queryValue.Contains("i=login"))
                {
                    skipUserValidation = true;
                }
            }


            if (!context.HttpContext.Request.Path.StartsWithSegments(new PathString("/api/User/Validate")) && !skipUserValidation)
            {

                var user = _dbContext.Users.Find(userId);
                if (user == null)
                    return;

                if (user.DeviceId != deviceId.ToString())
                {
                    context.Result = new UnauthorizedObjectResult("$_Duplicated_Session");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

}
