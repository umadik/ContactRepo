using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactMicroservices.Services.Contact.Model
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("company")]
        public string Company { get; set; }

        [BsonElement("infoTypes")]
        public List<InfoType> InfoTypes { get; set; } = new List<InfoType>();
    }
}
