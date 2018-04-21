using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public class ProductContext : ItemContext<Product>
    {
        protected override string CollectionName => "Items";
        
        public override async Task<IEnumerable<Product>> GetAll(string query = null)
        {
            var builder = new FilterDefinitionBuilder<Product>();
            var filter = builder.Empty;

            if(!String.IsNullOrWhiteSpace(query))
            {
                filter = filter & builder.Regex(x => x.Name, BsonRegularExpression.Create(new Regex(query, RegexOptions.IgnoreCase)));
            }

            return await Items.Find(filter).ToListAsync();
        }
    }
}
