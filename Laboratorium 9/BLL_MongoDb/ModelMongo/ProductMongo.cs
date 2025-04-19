using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_MongoDb.ModelMongo
{
    public class ProductMongo
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Price")]
        public double Price { get; set; }

        [BsonElement("Image")]
        public string? Image { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("GroupId")]
        public int GroupId { get; set; }
    }
}
