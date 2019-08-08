using CentralizedDataSystem.Models;
using CentralizedDataSystem.Repositories.Interfaces;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class FormControlService : IFormControlService {
        private readonly IMongoRepository _mongoRepository;

        public FormControlService(IMongoRepository mongoRepository) {
            _mongoRepository = mongoRepository;
            _mongoRepository.InitCollection(Collections.FORMS_CONTROL);
        }

        private FormControl CreateFormControlFromBson(BsonDocument document) {
            string pathForm = document[Keywords.PATH_FORM].ToString();
            string owner = document[Keywords.OWNER].ToString();
            string assign = document[Keywords.ASSIGN].ToString();
            string start = document[Keywords.START].ToString();
            string expired = document[Keywords.EXPIRED].ToString();

            return new FormControl(pathForm, owner, assign, start, expired);
        }

        public async Task<FormControl> FindByPathForm(string pathForm) {
            IAsyncCursor<BsonDocument> result = await _mongoRepository.FindBy(Keywords.PATH_FORM, pathForm);
            FormControl formControl = null;

            if (result == null) return formControl;

            using (IAsyncCursor<BsonDocument> cursor = result) {
                while (await cursor.MoveNextAsync()) {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    formControl = CreateFormControlFromBson(batch.First());
                    break;
                }
            }

            return formControl;
        }

        public async Task<List<FormControl>> FindByOwner(string email) {
            IAsyncCursor<BsonDocument> result = await _mongoRepository.FindBy(Keywords.OWNER, email);
            List<FormControl> formControls = new List<FormControl>();

            if (result == null) return formControls;

            using (IAsyncCursor<BsonDocument> cursor = result) {
                while (await cursor.MoveNextAsync()) {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch) {
                        formControls.Add(CreateFormControlFromBson(document));
                    }
                }
            }

            return formControls;
        }

        public bool Insert(FormControl formControl) {
            BsonDocument document = new BsonDocument {
                { Keywords.PATH_FORM, formControl.PathForm },
                { Keywords.OWNER, formControl.Owner },
                { Keywords.ASSIGN, formControl.Assign },
                { Keywords.START, formControl.Start},
                { Keywords.EXPIRED, formControl.Expired }
            };

            return _mongoRepository.Insert(document);
        }

        public async Task<long> Update(FormControl formControl, string oldPath) {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(Keywords.PATH_FORM, oldPath);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                                                    .Set(Keywords.PATH_FORM, formControl.PathForm)
                                                    .Set(Keywords.ASSIGN, formControl.Assign)
                                                    .Set(Keywords.START, formControl.Start)
                                                    .Set(Keywords.EXPIRED, formControl.Expired);

            long updateResult = await _mongoRepository.Update(filter, update);
            return updateResult;
        }

        public async Task<List<FormControl>> FindByAssign(string assign) {
            IAsyncCursor<BsonDocument> result = await _mongoRepository.FindBy(Keywords.ASSIGN, assign);
            List<FormControl> formControls = new List<FormControl>();

            if (result == null) return formControls;

            using (IAsyncCursor<BsonDocument> cursor = result) {
                while (await cursor.MoveNextAsync()) {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch) {
                        formControls.Add(CreateFormControlFromBson(document));
                    }
                }
            }

            return formControls;
        }

        public async Task<bool> DeleteByPathForm(string pathForm) {
            bool deleteResult = await _mongoRepository.DeleteBy(Keywords.PATH_FORM, pathForm);
            return deleteResult;
        }
    }
}