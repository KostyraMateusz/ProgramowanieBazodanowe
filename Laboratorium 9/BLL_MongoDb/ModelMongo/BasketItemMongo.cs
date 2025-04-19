using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_MongoDb.ModelMongo
{
    public  class BasketItemMongo
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("ProductId")]
        public int ProductId { get; set; }

        [BsonElement("UserId")]
        public int UserId { get; set; }

        [BsonElement("Amount")]
        public int Amount { get; set; }
    }
}
