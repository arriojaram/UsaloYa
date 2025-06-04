using Microsoft.EntityFrameworkCore;
using Serilog;
using UsaloYa.API.Security;
using UsaloYa.Library.Config;
using UsaloYa.Services;
using UsaloYa.Services.interfaces;


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

builder.Services.AddDbContext<UsaloYa.Library.Models.DBContext>(
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
builder.Services.AddSingleton<AppConfig>();
builder.Services.AddScoped<ProductCategoryService>();
builder.Services.AddScoped<AccessValidationFilter>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGeneralService, GeneralService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IEmailService, EmailService>();



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
