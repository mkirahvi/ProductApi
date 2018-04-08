namespace ProductsApi.Models
{
    public class MonitoredProduct
    {
        public string ProductId { get; set; }

        public NotificationSettings NotificationSettings { get; set; }
    }
}
