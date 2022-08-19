using MongoDB.Bson.Serialization.Attributes;


namespace DiChoThue.StoreService.Models
{
    public class ImageProduct
    {
        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }
    }
}
