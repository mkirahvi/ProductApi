using ProductsApi.Models;
using System.Collections.Generic;

namespace ProductsApi.Repositories
{
    public interface IProductRepository
    {
        void Add( Product item );
        IEnumerable<Product> GetAll();
        Product Find( string id );
        void Remove( string id );
        void Update( Product item );
    }
}
