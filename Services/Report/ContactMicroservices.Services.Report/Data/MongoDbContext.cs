using ContactMicroservices.Services.Report.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContactMicroservices.Services.Report.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<ReportDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Model.Report> Reports => _database.GetCollection<Model.Report>("Reports");
    }
}
