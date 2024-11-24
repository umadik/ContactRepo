using ContactMicroservices.Web.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContactMicroservices.Web.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoDatabase _reportsDatabase;

        public MongoDbContext(IOptions<ContactDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
            _reportsDatabase = client.GetDatabase(settings.Value.ReportsDatabaseName);
        }

        public IMongoCollection<Contact> Contacts => _database.GetCollection<Contact>("Contacts");
        public IMongoCollection<Report> Reports => _reportsDatabase.GetCollection<Report>("Reports");
    }
}
