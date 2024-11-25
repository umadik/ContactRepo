using ContactMicroservices.Services.Report.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContactMicroservices.Services.Report.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoDatabase _contactDatabase;
        
        public MongoDbContext(IOptions<ReportDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
            _contactDatabase = client.GetDatabase(settings.Value.ContactsDatabaseName);

        }

        public virtual IMongoCollection<Model.Report> Reports => _database.GetCollection<Model.Report>("Reports");
        public virtual IMongoCollection<Contact> Contacts => _contactDatabase.GetCollection<Contact>("Contacts");

    }
}
