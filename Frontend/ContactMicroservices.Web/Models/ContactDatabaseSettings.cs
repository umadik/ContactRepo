namespace ContactMicroservices.Web.Models
{
    public class ContactDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContactCollectionName { get; set; }
        public string ReportsDatabaseName { get; set; }  // Eksik olan bu!

    }
}
