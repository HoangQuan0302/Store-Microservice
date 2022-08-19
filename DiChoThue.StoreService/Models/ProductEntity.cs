using DiChoThue.StoreService.Enum;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DiChoThue.StoreService.Models
{
    public class ProductEntity
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("StoreId")]
        public string StoreId { get; set; }

        [BsonElement("ProductName")]
        public string Name { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }
        [BsonElement("Images")]
        public string ImagesUrl { get; set; }

        [BsonElement("Quantity")]
        public decimal? Quantity { get; set; }
        [BsonElement("Price")]
        public decimal? Price { get; set; }
    }
}
