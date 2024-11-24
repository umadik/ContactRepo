namespace ContactMicroservices.Services.Report.Model
{
    public class ReportDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ReportCollectionName { get; set; }
        public string ContactCollectionName { get; set; }
        public string ContactsDatabaseName { get; set; }
    }
}
