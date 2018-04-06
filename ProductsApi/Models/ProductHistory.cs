using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ProductsApi.Models
{
    public class ProductHistory
    {
        [BsonElement( "price" )]
        public decimal Price { get; set; }

        [BsonElement( "date" )]
        public string Date { get; set; }
    }
}
