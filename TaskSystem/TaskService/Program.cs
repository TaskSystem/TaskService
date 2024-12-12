using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using TaskService.MongoDB;
using TaskService.PublishRMQ;
using TaskService.Repository;

var builder = WebApplication.CreateBuilder(args);

// Registreer de RabbitMQ ConnectionFactory
builder.Services.AddSingleton<ConnectionFactory>(sp => new ConnectionFactory
{
    HostName = "rabbitmq",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
});

// Registreer de RabbitMQ IConnection als Singleton met Retry Mechanisme
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = sp.GetRequiredService<ConnectionFactory>();

    int retries = 20; // Aantal retries
    while (retries > 0)
    {
        try
        {
            var connection = factory.CreateConnection();
            Console.WriteLine("Connected to RabbitMQ");
            return connection;
        }
        catch (BrokerUnreachableException ex)
        {
            retries--;
            Console.WriteLine($"Failed to connect to RabbitMQ. Retries left: {retries}");
            if (retries == 0)
                throw new Exception("Could not connect to RabbitMQ after multiple retries.", ex);
            Thread.Sleep(2000); // Wacht 2 seconden voor de volgende poging
        }
    }

    throw new InvalidOperationException("RabbitMQ connection could not be established.");
});


builder.Services.AddSingleton<MongoDBService>();

// Publisher en Consumer registreren
builder.Services.AddSingleton<PublisherService>();
builder.Services.AddSingleton<Consumer>();

// Add services to the container.
builder.Services.AddSingleton<ITaskRepository, TaskRepository>();
builder.Services.AddControllers();



// Swagger configuratie
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task API", Version = "v1" });
});
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();



var app = builder.Build();

// Zorg ervoor dat routing is ingeschakeld
app.UseRouting();



app.UseSwagger();
app.UseSwaggerUI();
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Start de RabbitMQ-consumer na het bouwen van de applicatie
var consumer = app.Services.GetRequiredService<Consumer>();
consumer.StartListening();

app.Run();

