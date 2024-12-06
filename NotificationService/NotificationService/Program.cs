using NotificationService;

var builder = WebApplication.CreateBuilder(args);

// Configure SendGrid API Key and other environment variables
var sendGridApiKey = builder.Configuration["SENDGRID_API_KEY"];
if (string.IsNullOrEmpty(sendGridApiKey))
{
    throw new Exception("SendGrid API Key is not configured. Set it as an environment variable or in appsettings.json.");
}
builder.Services.AddSingleton(new EmailService(sendGridApiKey));

// Register the NotificationService as a hosted service
builder.Services.AddHostedService<NotificationService.NotificationService>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
