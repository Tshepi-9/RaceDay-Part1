using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers.
builder.Services.AddControllers();

// Register Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQL Server database connection.
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Enable session storage.
builder.Services.AddDistributedMemoryCache();

// Configure session management.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register password hashing.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register Azure Blob Storage service.
builder.Services.AddScoped<AzureBlobService>();

var app = builder.Build();

// Configure Swagger during development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enable session management.
app.UseSession();

// Enable authorization.
app.UseAuthorization();

// Map controller endpoints.
app.MapControllers();

// Start the application.
app.Run();