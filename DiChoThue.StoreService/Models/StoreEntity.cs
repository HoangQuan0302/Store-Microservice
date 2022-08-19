using DiChoThue.StoreService.Enum;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DiChoThue.StoreService.Models
{
    public class StoreEntity
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        //Information supply for approved process
        [BsonElement("BusinessLicense")]
        public string BusinessLicense { get; set; }

        [BsonElement("FoodSafetyCert")]
        public string FoodSafetyCert { get; set; }

        //Information After Approved by product owner Application

        [BsonElement("NameStore")]
        public string NameStore { get; set; }
        [BsonElement("AddressStore")]
        public string AddressStore { get; set; }

        [BsonElement("ProductType")]
        public List<ProductCategoryEntity> ProductCategory { get; set; }

        [BsonElement("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("EmployeeName")]
        public string EmployeeName { get; set; }

        [BsonElement("ImageStore")]
        public List<ImageStoreEntity> ImageStore { get; set; }

        [BsonElement("OpenTime")]
        public string OpenTime { get; set; }
        [BsonElement("CloseTime")]
        public string CloseTime { get; set; }
        [BsonElement("CloseDate")]
        public List<DateTime> CloseDate { get; set; }
        [BsonElement("FoundingDate")]
        public DateTime FoundingDate { get; set; }

        [BsonElement("Contract")]
        public string Contract { get; set; }

        [BsonElement("Status")]
        public StatusStore Status { get; set; }

        //User Infomation
        [BsonElement("UserId")]
        public string UserId { get; set; }
    }
}
