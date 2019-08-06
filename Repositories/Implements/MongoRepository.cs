using CentralizedDataSystem.Repositories.Interfaces;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using CentralizedDataSystem.Resources;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace CentralizedDataSystem.Repositories.Implements {
    public class MongoRepository : IMongoRepository {
        private MongoClient _mongoClient;
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _collection;

        public void InitCollection(string collectionName) {
            _mongoClient = new MongoClient(Configs.MONGO_CONNECTION_STRING);
            _database = _mongoClient.GetDatabase(Configs.DATABASE_NAME);
            _collection = _database.GetCollection<BsonDocument>(collectionName);
        }

        public async Task<bool> DeleteBy(string field, string value) {
            try {
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(field, value);
                DeleteResult deleteResult = await _collection.DeleteOneAsync(filter);

                return deleteResult.DeletedCount > 0;
            } catch (Exception) {
                return false;
            }
        }

        public async Task<IAsyncCursor<BsonDocument>> FindBy(string field, string value) {
            try {
                FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
                FilterDefinition<BsonDocument> filter = builder.Eq(field, value);
                return await _collection.FindAsync(filter);
            } catch (Exception) {
                return null;
            }
        }

        public bool Insert(BsonDocument document) {
            try {
                _collection.InsertOneAsync(document).Wait();
                return true;
            } catch (AggregateException) {
                return false;
            }
        }

        public async Task<long> Update(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update) {
            try {
                UpdateResult updateResult = await _collection.UpdateOneAsync(filter, update);

                return updateResult.ModifiedCount;
            } catch (Exception) {
                return -1;
            }
        }
    }
}