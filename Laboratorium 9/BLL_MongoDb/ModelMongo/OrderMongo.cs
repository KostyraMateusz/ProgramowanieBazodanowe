using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_MongoDb.ModelMongo
{
    public class OrderMongo
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("UserId")]
        public int UserId { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [BsonElement("IsPaid")]
        public bool IsPaid { get; set; } = false;

        [BsonElement("OrderPositions")]
        public List<OrderItemMongo> OrderPositions { get; set; } = new();
    }
}
