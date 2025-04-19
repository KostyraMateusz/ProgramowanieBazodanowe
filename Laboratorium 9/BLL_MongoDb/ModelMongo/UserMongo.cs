using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_MongoDb.ModelMongo
{
    public class UserMongo
    {
        public class MongoUser
        {
            [BsonId]
            public int Id { get; set; }

            [BsonElement("Login")]
            public string Login { get; set; }

            [BsonElement("Password")]
            public string Password { get; set; }

            [BsonElement("Type")]
            public UserType Type { get; set; } = UserType.Casual;

            [BsonElement("IsActive")]
            public bool IsActive { get; set; } = true;

            [BsonElement("GroupId")]
            public int? GroupId { get; set; }
        }

        public enum UserType
        {
            Admin,
            Casual
        }
    }
}
