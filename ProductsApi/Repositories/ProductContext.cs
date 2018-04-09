using MongoDB.Bson;
using MongoDB.Driver;
using ProductsApi.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProductsApi.Repositories
{
    public class ProductContext
    {
        IMongoDatabase database; 

        public ProductContext()
        {
            string connectionString = "mongodb://superAdmin:admin123@88.206.52.198:27017";
            var connection = new MongoUrlBuilder( connectionString);
            
            MongoClient client = new MongoClient( connectionString );
            database = client.GetDatabase( "Catalog" );
        }

        private IMongoCollection<Product> Products
        {
            get { return database.GetCollection<Product>( "Items" ); }
        }

        public async Task<IEnumerable<Product>> GetProducts(string query = null)
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<Product>();
            var filter = builder.Empty;

            if( !String.IsNullOrWhiteSpace( query ) )
            {
                filter = filter & builder.Regex( x => x.Name, BsonRegularExpression.Create(new Regex( query, RegexOptions.IgnoreCase ) ) );
            }

            return await Products.Find( filter ).ToListAsync();
        }

        // получаем один документ по id
        public async Task<Product> GetProduct( string id )
        {
            return await Products.Find( new BsonDocument( "_id", new ObjectId( id ) ) ).FirstOrDefaultAsync();
        }
        // добавление документа
        public async Task Create( Product p )
        {
            await Products.InsertOneAsync( p );
        }
        // обновление документа
        public async Task Update( Product p )
        {
            await Products.ReplaceOneAsync( new BsonDocument( "_id", new ObjectId( p.Id ) ), p );
        }
        // удаление документа
        public async Task Remove( string id )
        {
            await Products.DeleteOneAsync( new BsonDocument( "_id", new ObjectId( id ) ) );
        }
    }
}
