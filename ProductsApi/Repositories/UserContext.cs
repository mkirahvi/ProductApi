using MongoDB.Bson;
using MongoDB.Driver;
using ProductsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsApi.Repositories
{
    public class UserContext
    {
        IMongoDatabase database; 

        public UserContext()
        {
            string connectionString = "mongodb://superAdmin:admin123@88.206.52.198:27017";
            var connection = new MongoUrlBuilder( connectionString);
            
            MongoClient client = new MongoClient( connectionString );
            database = client.GetDatabase( "Catalog" );
        }

        private IMongoCollection<User> Users
        {
            get { return database.GetCollection<User>( "users" ); }
        }

        public async Task<IEnumerable<User>> GetUsers( )
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<User>();
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

            return await Users.Find( filter ).Limit(10).ToListAsync();
        }

        // получаем один документ по id
        public async Task<User> GetUser( string id )
        {
            return await Users.Find( new BsonDocument( "_id", new ObjectId( id ) ) ).FirstOrDefaultAsync();
        }
        // добавление документа
        public async Task Create( User p )
        {
            await Users.InsertOneAsync( p );
        }
        // обновление документа
        public async Task Update( User p )
        {
            await Users.ReplaceOneAsync( new BsonDocument( "_id", new ObjectId( p.Id ) ), p );
        }
        // удаление документа
        public async Task Remove( string id )
        {
            await Users.DeleteOneAsync( new BsonDocument( "_id", new ObjectId( id ) ) );
        }
    }
}
