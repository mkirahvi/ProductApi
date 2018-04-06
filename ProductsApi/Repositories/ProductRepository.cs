using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private static ConcurrentDictionary<string, Product> _products =
              new ConcurrentDictionary<string, Product>();

        public ProductRepository()
        {
            Add( new Product { Name = "Item1" } );
        }

        public void Add( Product item )
        {
            item.Id = Guid.NewGuid().ToString();
            _products[item.Id] = item;
        }

        public Product Find( string id )
        {
            Product item;
            _products.TryGetValue( id, out item );
            return item;
        }

        public IEnumerable<Product> GetAll()
        {
            return _products.Values;
        }

        public void Remove( string id )
        {
            Product item;
            _products.TryRemove( id, out item );
        }

        public void Update( Product item )
        {
            _products[item.Id] = item;
        }
    }
}
