using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductsApi.Models
{
    public class Product
    {
        [BsonRepresentation( BsonType.ObjectId )]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Link { get; set; }

        public bool Available { get; set; }

        public IEnumerable<ProductHistory> History { get; set; }
    }
}
