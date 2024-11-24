using MongoDB.Bson.Serialization.Attributes;

namespace ContactMicroservices.Web.Models
{
    public class InfoType
    {
        [BsonElement("InfoValueType")]
        public InfoValueType Type { get; set; }

        [BsonElement("Value")]
        public string Value { get; set; }
    }
}
