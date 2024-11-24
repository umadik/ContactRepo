using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactMicroservices.Web.Models
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("status")]
        public ReportStatus Status { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("contactCount")]
        public int ContactCount { get; set; }

        [BsonElement("phoneNumberCount")]
        public int PhoneNumberCount { get; set; }
    }
}
