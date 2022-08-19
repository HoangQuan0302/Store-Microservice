using MongoDB.Bson.Serialization.Attributes;

namespace DiChoThue.StoreService.Models
{
    public class ProductCategoryEntity
    {
        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }
        [BsonElement("CategoryName")]
        public string CategoryName { get; set; }
    }
}
 