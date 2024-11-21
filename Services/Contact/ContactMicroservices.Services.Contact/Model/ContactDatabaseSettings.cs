namespace ContactMicroservices.Services.Contact.Model
{
    public class ContactDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContactCollectionName { get; set; }
    }
}
