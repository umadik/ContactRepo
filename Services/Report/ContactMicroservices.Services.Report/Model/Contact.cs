using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ContactMicroservices.Services.Report.Model
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("Company")]
        public string Company { get; set; }

        [BsonElement("InfoTypes")]
        public List<InfoType> InfoTypes { get; set; } = new List<InfoType>();

    }
}
