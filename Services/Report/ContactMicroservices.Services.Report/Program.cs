using ContactMicroservices.Services.Report.Data;
using ContactMicroservices.Services.Report.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// MongoDB ayarlarýný yapýlandýr
builder.Services.Configure<ReportDatabaseSettings>(
    builder.Configuration.GetSection("ReportDatabaseSettings"));

// MongoDbContext'i DI container'a ekle
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddHttpClient();

builder.Services.AddCap(options =>
{
    options.UseRabbitMQ(options =>
    {
        options.ConnectionFactoryOptions = options =>
        {
            options.Ssl.Enabled = false;
            options.HostName = "localhost";
            options.UserName = "guest";
            options.Password = "guest";
            options.Port = 5672;
        };
    });

    options.UseMongoDB(mongodbOptions =>
    {
        mongodbOptions.DatabaseConnection = "mongodb://localhost:27017";
        mongodbOptions.DatabaseName = "RabbitMQDB";
    });
});

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
