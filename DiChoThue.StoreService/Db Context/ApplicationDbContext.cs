using DiChoThue.StoreService.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DiChoThue.StoreService.Db_Context
{
    public class ApplicationDbContext:IApplicationDbContext
    {
        public ApplicationDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
            ListStores = database.GetCollection<StoreEntity>(configuration.GetValue<string>("DatabaseSettings:CollectionStore"));
            ListProducts = database.GetCollection<ProductEntity>(configuration.GetValue<string>("DatabaseSettings:CollectionProduct"));
            ApplicationDbContextSeed.SeedData(ListStores,ListProducts);
        }
        public IMongoCollection<StoreEntity> ListStores { get; }
        public IMongoCollection<ProductEntity> ListProducts { get; }
    }
}
