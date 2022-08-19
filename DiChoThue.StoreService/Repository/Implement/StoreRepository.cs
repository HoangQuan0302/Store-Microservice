using DiChoThue.StoreService.Db_Context;
using DiChoThue.StoreService.Models;
using DiChoThue.StoreService.Repository.Interface;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DiChoThue.StoreService.Repository.Implement
{
    public class StoreRepository : IStoreRepository
    {
        private readonly IApplicationDbContext _storeService;

        public StoreRepository(IApplicationDbContext applicationDbContext)
        {
            _storeService = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }
        public async Task RegisterStore(StoreEntity _objStore)
        {
            await _storeService.ListStores.InsertOneAsync(_objStore);
        }

        public async Task<StoreEntity> GetStore(string UserId)
        {
            return await _storeService
                                                .ListStores
                                                .Find(s => s.UserId == UserId)
                                                .FirstOrDefaultAsync();

        }
    }
}
