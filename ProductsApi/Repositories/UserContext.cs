using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public class UserContext : ItemContext<User>
    {
        protected override string CollectionName => "users";
        
        public override async Task<IEnumerable<User>> GetAll(string query = null)
        {
            var builder = new FilterDefinitionBuilder<User>();
            var filter = builder.Empty;

            if(!String.IsNullOrWhiteSpace(query))
            {
                filter = filter & builder.Eq(u => u.Name, query);
            }

            return await Items.Find(filter).ToListAsync();;
        }
    }
}
