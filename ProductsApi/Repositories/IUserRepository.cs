using ProductsApi.Models;
using System.Collections.Generic;

namespace ProductsApi.Repositories
{
    public interface IUserRepository
    {
        void Add( User item );
        IEnumerable<User> GetAll();
        User Find( string id );
        void Remove( string id );
        void Update( User item );
    }
}
