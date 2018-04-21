using System.Collections.Generic;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public class MongoProductRepository : Repository<Product>, IRepository<Product>
    {
        public MongoProductRepository()
        {
            _context = new ProductContext();
        }
    }
}
