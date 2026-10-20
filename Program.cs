using Microsoft.EntityFrameworkCore;
using OliveOilApi.Data;

var builder = WebApplication.CreateBuilder(args);

// ===== Services =====
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));

    // Development only: print parameter VALUES in the SQL log (to trace the queries).
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();   // http://localhost:5149/swagger
}

app.MapControllers();

app.Run();
