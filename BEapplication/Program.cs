using BEapplication.DBContexts;
using BEapplication.Interfaces;
using BEapplication.RequestHandlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer("Server=(localdb)\\Local;Database=MyApplicationDB;Trusted_Connection=True;");
});
builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<IReservationLogic, ReservationLogic>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind CORS policy from appsettings.json
builder.Services.AddCors(options =>
{
    var configuration = builder.Configuration;
    options.AddPolicy("AllowLocalhost3000", builder =>
    {
        builder
            .WithOrigins(configuration["Cors:AllowLocalhost3000:Origins"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost3000"); // Apply the CORS policy using the policy name
app.UseAuthorization();
app.MapControllers();
app.Run();
