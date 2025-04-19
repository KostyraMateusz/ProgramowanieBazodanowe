using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_MongoDb.ServiceMongo
{
    public class SequenceService
    {
        private readonly IMongoCollection<BsonDocument> _counters;

        public SequenceService(IMongoDatabase db)
        {
            _counters = db.GetCollection<BsonDocument>("counters");
        }

        public int GetNextSequence(string name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", name);
            var update = Builders<BsonDocument>.Update.Inc("sequence_value", 1);

            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var result = _counters.FindOneAndUpdate(filter, update, options);
            return result["sequence_value"].AsInt32;
        }
    }
}
