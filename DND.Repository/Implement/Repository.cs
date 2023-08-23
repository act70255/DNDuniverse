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
    public class Repository<T> : IRepository<T>
    {
        IMongoCollection<T> _collection;
        IMongoDatabase _mongoDB;

        public Repository(IMongoDatabase mongoDB)
        {
            _mongoDB = mongoDB;
            _collection = _mongoDB.GetCollection<T>(typeof(T).Name);//..Collection<T>(dbName, typeof(T).Name);
        }

        public IEnumerable<T> GetAll()
        {
            return _collection.Find(_ => true).ToList();
        }

        public T GetByID(int id)
        {
            var filter = Builders<T>.Filter.Eq("ID", id);
            return _collection.Find(filter).FirstOrDefault();
        }

        public void Insert(T entity)
        {
            _collection.InsertOne(entity);
        }

        public void Update(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                var filter = Builders<T>.Filter.Eq("ID", baseEntity.ID);
                _collection.ReplaceOne(filter, entity);
            }
        }

        public void Delete(int id)
        {
            var filter = Builders<T>.Filter.Eq("ID", id);
            _collection.DeleteOne(filter);
        }
    }
}
