using System;

namespace ProductsApi.Models
{
    public class MonitoredProduct
    {
        public Guid ProductId { get; set; }

        public NotificationSettings NotificationSettings { get; set; }
    }
}
