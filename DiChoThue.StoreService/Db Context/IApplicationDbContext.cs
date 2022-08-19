using DiChoThue.StoreService.Models;
using MongoDB.Driver;

namespace DiChoThue.StoreService.Db_Context
{
    public interface IApplicationDbContext
    {
        IMongoCollection<StoreEntity> ListStores { get; }
        IMongoCollection<ProductEntity> ListProducts { get; }
    }
}
