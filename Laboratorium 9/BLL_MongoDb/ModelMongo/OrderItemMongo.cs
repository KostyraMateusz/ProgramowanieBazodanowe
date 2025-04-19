using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_MongoDb.ModelMongo
{
    public class OrderItemMongo
    {
        [BsonElement("ProductId")]
        public int ProductId { get; set; }

        [BsonElement("ProductName")]
        public string ProductName { get; set; }

        [BsonElement("Price")]
        public double Price { get; set; }

        [BsonElement("Amount")]
        public int Amount { get; set; }
    }
}
