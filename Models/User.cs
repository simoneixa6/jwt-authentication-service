using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace jwt_authentication_service.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string username { get; set; }
        [JsonIgnore]
        [Required]
        public string password { get; set; }
    }
}
