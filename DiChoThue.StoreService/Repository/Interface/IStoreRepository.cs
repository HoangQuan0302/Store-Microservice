using DiChoThue.StoreService.Models;
using System.Threading.Tasks;

namespace DiChoThue.StoreService.Repository.Interface
{
    public interface IStoreRepository
    {
        Task RegisterStore(StoreEntity _objStore);
        Task<StoreEntity> GetStore(string UserId);
    }
}
