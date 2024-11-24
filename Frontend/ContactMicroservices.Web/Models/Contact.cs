using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ContactMicroservices.Web.Models
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public List<InfoType> InfoTypes { get; set; } = new List<InfoType>();
    }
}
