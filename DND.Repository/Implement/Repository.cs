using DND.Model.Entity;
using DND.Repository.Interface;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Repository.Implement
{
    public class Repository<TDocument> : IRepository<TDocument>
    {
        IMongoCollection<TDocument> _collection;
        IMongoDatabase _mongoDB;

        public Repository(IMongoDatabase mongoDB)
        {
            _mongoDB = mongoDB;
            _collection = _mongoDB.GetCollection<TDocument>(typeof(TDocument).Name);//..Collection<T>(dbName, typeof(T).Name);
        }

        public IEnumerable<TDocument> GetAll()
        {
            return _collection.Find(_ => true).ToList();
        }

        public TDocument GetByID(int id)
        {
            var filter = Builders<TDocument>.Filter.Eq("ID", id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public void Insert(TDocument entity)
        {
            _collection.InsertOne(entity);
        }

        public void Update(TDocument entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                var filter = Builders<TDocument>.Filter.Eq("ID", baseEntity.ID);
                _collection.ReplaceOne(filter, entity);
            }
        }

        public void Delete(int id)
        {
            var filter = Builders<TDocument>.Filter.Eq("ID", id);
            _collection.DeleteOne(filter);
        }
    }
}
