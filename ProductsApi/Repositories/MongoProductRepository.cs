using System.Collections.Generic;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public class MongoProductRepository : IProductRepository
    {
        private ProductContext _context = new ProductContext();
        

        public void Add( Product item )
        {
             _context.Create( item ).Wait();
        }

        public Product Find( string id )
        {
            Product item = _context.GetProduct(id).Result;
            return item;
        }

        public IEnumerable<Product> GetAll(string query = null)
        {
            return _context.GetProducts(query).Result;
        }

        public void Remove( string id )
        {
            _context.Remove( id ).Wait();
        }

        public void Update( Product item )
        {
            _context.Update( item ).Wait();
        }
    }
}
