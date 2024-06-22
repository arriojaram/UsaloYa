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

            var appToken = _configuration.GetValue<string>("ApiKey");

            if (!appToken.Equals(extractedToken))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Not Authorized");
                return;
            }

            await _next(context);
        }
    }

}
