using DocumentRegisteryAppApi.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<AppDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOriginsPolicy",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAllOriginsPolicy");
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<AppDbContext>();
await context.Database.MigrateAsync();

app.MapGet("health", () =>
{
    return Results.Ok("Service is healthy!");
});

app.Run();
