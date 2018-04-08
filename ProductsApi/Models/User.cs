using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ProductsApi.Models
{
    public class User
    {
        [BsonRepresentation( BsonType.ObjectId )]
        public string Id { get; set; }
        
        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public IEnumerable<MonitoredProduct> MonitoredProducts { get; set; }
    }
}
