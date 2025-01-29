using System.Text;
using System.Text.Json;
using UsaloYa.API.DTO;
using UsaloYa.API.Models;

namespace UsaloYa.API.Security
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var extractedToken))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var path = context.Request.Path;
            if (path.HasValue && path.Value == "/api/User/Validate")
            {

            }
            else
            {
                context.Request.Headers.TryGetValue("RequestorId", out var requestorIdStr);
                if (!int.TryParse(requestorIdStr, out _))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Usuario no reconocido.");
                    return;
                }

                context.Request.Headers.TryGetValue("DeviceId", out var deviceId);
                if (string.IsNullOrEmpty(deviceId))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Dispositivo no reconocido.");
                    return;
                }
                

                var appToken = _configuration.GetValue<string>("ApiKey") ?? "";

                if (!appToken.Equals(extractedToken))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Acceso no autorizado.");
                    return;
                }
            }
            await _next(context);
        }
    }

}
