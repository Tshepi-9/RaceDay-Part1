using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Register controllers.
builder.Services.AddControllers();

// Register Swagger for API documentation and testing.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the SQL Server database connection.
builder.Services.AddDbContext<RaceDayDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")));

// Enable in-memory session storage.
builder.Services.AddDistributedMemoryCache();

// Configure session management.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register password hashing for user accounts.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure Swagger during development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enable session management before the controllers run.
app.UseSession();

// Enable authorization.
app.UseAuthorization();

// Map controller endpoints.
app.MapControllers();

// Start the application.
app.Run();
