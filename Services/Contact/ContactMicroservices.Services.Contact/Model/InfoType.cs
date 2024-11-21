using MongoDB.Bson.Serialization.Attributes;

namespace ContactMicroservices.Services.Contact.Model
{
    public class InfoType
    {
        [BsonElement("type")]
        public string Type { get; set; } // Örn: Telefon Numarası, Email, Konum

        [BsonElement("value")]
        public string Value { get; set; } // Örn: +905555555, email@email.com

    }
}
