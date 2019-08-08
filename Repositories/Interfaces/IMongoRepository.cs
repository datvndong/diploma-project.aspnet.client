using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Repositories.Interfaces {
    public interface IMongoRepository {
        void InitCollection(string collectionName);
        Task<IAsyncCursor<BsonDocument>> FindBy(string field, string value);
        bool Insert(BsonDocument document);
        Task<long> Update(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update);
        Task<bool> DeleteBy(string field, string value);
    }
}
