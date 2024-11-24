using ContactMicroservices.Web.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContactMicroservices.Web.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<ContactDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Contact> Contacts => _database.GetCollection<Contact>("Contacts");
    }
}
