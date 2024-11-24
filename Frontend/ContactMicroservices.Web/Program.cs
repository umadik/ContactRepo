using ContactMicroservices.Web.Data;
using ContactMicroservices.Web.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// HttpClient'ý DI'ye ekle
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5011/api/"); // API'nin base adresi
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// MongoDB Connection
// MongoDB ayarlarýný yapýlandýr
builder.Services.Configure<ContactDatabaseSettings>(
    builder.Configuration.GetSection("ContactDatabaseSettings"));

builder.Services.AddScoped<MongoDbContext>();

// RabbitMQ - CAP Publisher
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

builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Contact/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contact}/{action=Index}/{id?}");

app.Run();
