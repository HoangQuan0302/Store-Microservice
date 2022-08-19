using MongoDB.Bson.Serialization.Attributes;

namespace DiChoThue.StoreService.Models
{
    public class ImageStoreEntity
    {
        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }
    }
}
