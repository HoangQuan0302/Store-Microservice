using DiChoThue.StoreService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiChoThue.StoreService.Repository.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductEntity>> GetAllProduct();
        Task<ProductEntity> GetProductById(string id);
        Task<IEnumerable<ProductEntity>> GetProductsStore(string storeId);
        Task<IEnumerable<ProductEntity>> GetProductByCategory(string productId);
        Task CreateProduct(ProductEntity _product);
        Task<bool> UpdateProduct(ProductEntity _product);
        Task<bool> DeleteProduct(string _productId);
    }
}
