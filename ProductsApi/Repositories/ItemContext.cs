using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public abstract class ItemContext<TItem> where TItem:Item
    {
        protected IMongoDatabase database;

        protected abstract string CollectionName { get; }

        protected ItemContext()
        {
            string connectionString = "mongodb://superAdmin:admin123@88.206.52.198:27017";
            var connection = new MongoUrlBuilder(connectionString);

            MongoClient client = new MongoClient(connectionString);

            database = client.GetDatabase("Catalog");
        }

        protected IMongoCollection<TItem> Items
        {
            get { return database.GetCollection<TItem>(CollectionName); }
        }

        public async Task<TItem> Get(string id)
        {
            return await Items.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task Create(TItem p)
        {
            await Items.InsertOneAsync(p);
        }
    
        public async Task Update(TItem p)
        {
            await Items.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }

        public async Task Remove(string id)
        {
            await Items.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public abstract Task<IEnumerable<TItem>> GetAll(string query = null);

    }
}
