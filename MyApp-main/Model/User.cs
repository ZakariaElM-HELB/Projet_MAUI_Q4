using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyApp.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; } // ✅ Nouveau champ

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool CanBeDeleted { get; set; } // Utilisé uniquement dans la vue

    }

}
