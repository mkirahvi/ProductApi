using System.Collections.Generic;

namespace ProductsApi.Models
{
    public class NotificationSettings
    {
        public bool Availability { get; set; }

        public bool PriceChanging { get; set; }

        public decimal NecessaryPrice { get; set; }

        public bool Sign { get; set; }

        public IEnumerable<string> Shops { get; set; }
    }
}
