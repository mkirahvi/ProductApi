using System.Collections.Generic;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public class MongoUserRepository : Repository<User>, IRepository<User>
    {
        public MongoUserRepository()
        {
            _context = new UserContext();
        }
    }
}
