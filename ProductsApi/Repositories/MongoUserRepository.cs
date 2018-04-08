using System.Collections.Generic;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public class MongoUserRepository : IUserRepository
    {
        private UserContext _context = new UserContext();
        

        public void Add( User item )
        {
             _context.Create( item ).Wait();
        }

        public User Find( string id )
        {
            User item = _context.GetUser(id).Result;
            return item;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.GetUsers().Result;
        }

        public void Remove( string id )
        {
            _context.Remove( id ).Wait();
        }

        public void Update( User item )
        {
            _context.Update( item ).Wait();
        }
    }
}
