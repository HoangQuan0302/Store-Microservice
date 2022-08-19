using DiChoThue.StoreService.Db_Context;
using DiChoThue.StoreService.Models;
using DiChoThue.StoreService.Repository.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiChoThue.StoreService.Repository.Implement
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext _productService;

        public ProductRepository(IApplicationDbContext applicationDbContext)
        {
            _productService = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }
        public async Task CreateProduct(ProductEntity _product)
        {
            await _productService.ListProducts .InsertOneAsync(_product);
        }

        public async Task<bool> DeleteProduct(string _productId)
        {
            DeleteResult deleteResult = await _productService
                                                .ListProducts
                                                .DeleteOneAsync(p=>p.Id==_productId);

            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductEntity>> GetAllProduct()
        {
            return await _productService.ListProducts.Find(p => true).ToListAsync();
        }

        public async Task<IEnumerable<ProductEntity>> GetProductByCategory(string category)
        {
            return await _productService.ListProducts.Find(p=>p.Category==category).ToListAsync();
        }

        public async Task<IEnumerable<ProductEntity>> GetProductsStore(string storeId)
        {
            return await _productService.ListProducts.Find(p => p.StoreId == storeId).ToListAsync();
        }
        public async Task<ProductEntity> GetProductById(string id)
        {
            return await _productService.ListProducts.Find(p => p.Id==id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateProduct(ProductEntity _product)
        {
            var updateResult = await _productService
                                         .ListProducts
                                         .ReplaceOneAsync(filter: g => g.Id == _product.Id, replacement: _product);

            return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;
        }
    }
}
