using Microsoft.EntityFrameworkCore;
using Serilog;
using UsaloYa.API.Config;
using UsaloYa.API.Security;
using UsaloYa.API.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var loggerSettings = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();
//Add logging settings
builder.Logging.AddSerilog(loggerSettings);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UsaloYa.API.Models.DBContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

// Configure CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<List<string>>(); // List of allowed URLS through CONFIG file

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            if (allowedOrigins != null)
                builder.WithOrigins(allowedOrigins.ToArray())  // Add or remove URLs from appsettings
                    //.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

builder.Services.AddScoped<ProductCategoryService>();
builder.Services.AddSingleton<AppConfig>();
builder.Services.AddScoped<AccessValidationFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();
app.MapControllers();

app.UseMiddleware<TokenValidationMiddleware>();

app.Run();
