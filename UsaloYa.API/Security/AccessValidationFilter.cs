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

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId) || string.IsNullOrEmpty(deviceId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            if (!context.HttpContext.Request.Headers.TryGetValue("UserId", out var userId) || string.IsNullOrEmpty(deviceId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return;

            if (user.DeviceId != deviceId.ToString())
            {
                context.Result = new UnauthorizedResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

}
