using ProductsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
