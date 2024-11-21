using ContactMicroservices.Services.Contact.Data;
using ContactMicroservices.Services.Contact.Model;

var builder = WebApplication.CreateBuilder(args);

// MongoDB ayarlarýný yapýlandýr
builder.Services.Configure<ContactDatabaseSettings>(
    builder.Configuration.GetSection("ContactDatabaseSettings"));

// MongoDbContext'i DI container'a ekle
builder.Services.AddSingleton<MongoDbContext>();

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
