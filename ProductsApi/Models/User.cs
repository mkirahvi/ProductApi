using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductsApi.Models
{
    public class User : Item
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public ICollection<MonitoredProduct> MonitoredProducts { get; set; }
    }
}
