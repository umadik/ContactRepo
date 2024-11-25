using ContactMicroservices.Services.Contact.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContactMicroservices.Services.Contact.Data
{
    public class MongoDbContext
    {

        private readonly IMongoDatabase _database;
        
        public MongoDbContext(IOptions<ContactDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public virtual IMongoCollection<Model.Contact> Contacts => _database.GetCollection<Model.Contact>("Contacts");

    }
}
