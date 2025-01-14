using BoardService.DbContextApp;
using BoardService.Messaging;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<PublisherToRMQ>();
builder.Services.AddControllers();

// Voeg Prometheus metrics service toe
builder.Services.AddMetrics();

// Voeg CORS-configuratie toe
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<BoardContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

Console.WriteLine($"Using connection string: {builder.Configuration.GetConnectionString("DefaultConnection")}");


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Retry logica bij opstarten
var maxRetries = 15;
var delay = TimeSpan.FromSeconds(5);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BoardContext>();

    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            Console.WriteLine($"Attempt {attempt}: Trying to connect to the database...");
            context.Database.Migrate(); // Voer migraties uit
            Console.WriteLine("Database connection successful.");
            break; // Verlaat de loop als het lukt
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection failed: {ex.Message}");
            if (attempt == maxRetries)
            {
                throw; // Stop als alle pogingen zijn mislukt
            }
            System.Threading.Thread.Sleep(delay);
        }
    }
}


// Exposeer /metrics endpoint
app.MapMetrics();

app.UseHttpsRedirection();

// Gebruik de CORS-policy
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();

