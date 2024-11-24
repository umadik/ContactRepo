using MongoDB.Bson.Serialization.Attributes;
namespace ContactMicroservices.Services.Contact.Model
{
    public class InfoType
    {
        [BsonElement("InfoValueType")]
        public InfoValueType Type { get; set; } // Örn: Telefon Numarası, Email, Konum

        [BsonElement("Value")]
        public string Value { get; set; } // Örn: +905555555, email@email.com

    }
}
