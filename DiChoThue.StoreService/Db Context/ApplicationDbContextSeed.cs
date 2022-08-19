using DiChoThue.StoreService.Enum;
using DiChoThue.StoreService.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace DiChoThue.StoreService.Db_Context
{
    public class ApplicationDbContextSeed
    {
        public static void SeedData(IMongoCollection<StoreEntity> _listStoresCollection,IMongoCollection<ProductEntity> _listProductsCollection)
        {
            bool existStore = _listStoresCollection.Find(s => true).Any();
            if (!existStore)
            {
                _listStoresCollection.InsertManyAsync(GetPreconfiguredStores());
            }
            bool existProducts = _listProductsCollection.Find(s => true).Any();
            if(!existProducts)
            {
                _listProductsCollection.InsertManyAsync(GetPreconfiguredProducts());
            }
        }

        private static IEnumerable<StoreEntity> GetPreconfiguredStores()
        {
            return new List<StoreEntity>()
            {
                new StoreEntity()
                {
                     Id = "602d2149e773f2a3990b47f5",
                     BusinessLicense="",
                     FoodSafetyCert="",
                     NameStore ="Kichi Kichi",
                     AddressStore ="123 Trần Hưng Đạo, Phường 14, Quận 5, Tp.Hồ Chí Minh",
                     ProductCategory=new List<ProductCategoryEntity> {new ProductCategoryEntity() {ImageUrl="",CategoryName="Rau Củ"}, new ProductCategoryEntity() {ImageUrl="",CategoryName = "Hải Sản Tươi Sống" } },
                     PhoneNumber="0963691721",
                     EmployeeName="Nguyễn Văn A",
                     ImageStore=new List<ImageStoreEntity> {new ImageStoreEntity() {ImageUrl=""} },
                     OpenTime="08:00",
                     CloseTime="21:00",
                     CloseDate=new List<DateTime> { new DateTime(2022,06,17) , new DateTime(2022,06,18)  },
                     FoundingDate=new DateTime(2021,02,03),
                     Contract="",
                     Status=StatusStore.Open,
                     UserId="1"
                }
            };
        }

        private static IEnumerable<ProductEntity> GetPreconfiguredProducts()
        {
            return new List<ProductEntity>()
            {
                new ProductEntity()
                {
                    StoreId="602d2149e773f2a3990b47f5",
                    Name="Súp lơ xanh",
                    Category="Rau Củ",
                    Description="Súp lơ xuất xứ từ Đà Lạt",
                    ImagesUrl="",
                    Quantity=10,
                    Price=100000
                }
            };
        }
    }
}
