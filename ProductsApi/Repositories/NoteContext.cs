using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Phone = ProductsApi.Models.Product;

namespace ProductsApi.Repositories
{
    public class MobileContext
    {
        IMongoDatabase database; 

        public MobileContext()
        {
            string connectionString = "mongodb://superAdmin:admin123@88.206.52.198:27017/Catalog";
            var connection = new MongoUrlBuilder( connectionString );
            MongoClient client = new MongoClient( connectionString );
            database = client.GetDatabase( connection.DatabaseName );
        }

        private IMongoCollection<Phone> Phones
        {
            get { return database.GetCollection<Phone>( "Phones" ); }
        }

        public async Task<IEnumerable<Phone>> GetPhones( )
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<Phone>();
            var filter = builder.Empty; 
            
            //if( !String.IsNullOrWhiteSpace( name ) )
            //{
            //    filter = filter & builder.Regex( "Name", new BsonRegularExpression( name ) );
            //}
            //if( minPrice.HasValue )  // фильтр по минимальной цене
            //{
            //    filter = filter & builder.Gte( "Price", minPrice.Value );
            //}
            //if( maxPrice.HasValue )  // фильтр по максимальной цене
            //{
            //    filter = filter & builder.Lte( "Price", maxPrice.Value );
            //}

            return await Phones.Find( filter ).Limit(10).ToListAsync();
        }

        // получаем один документ по id
        public async Task<Phone> GetPhone( string id )
        {
            return await Phones.Find( new BsonDocument( "_id", new ObjectId( id ) ) ).FirstOrDefaultAsync();
        }
        // добавление документа
        public async Task Create( Phone p )
        {
            await Phones.InsertOneAsync( p );
        }
        // обновление документа
        public async Task Update( Phone p )
        {
            await Phones.ReplaceOneAsync( new BsonDocument( "_id", new ObjectId( p.Id ) ), p );
        }
        // удаление документа
        public async Task Remove( string id )
        {
            await Phones.DeleteOneAsync( new BsonDocument( "_id", new ObjectId( id ) ) );
        }
    }
}
