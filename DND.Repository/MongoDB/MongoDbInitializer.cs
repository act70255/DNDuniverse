using DND.Model.Entity;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Repository.MongoDB
{
    public interface IDbInitializer
    {
        void EnsureSeedData();
    }
    public class MongoDbInitializer : IDbInitializer
    {
        private readonly string _connection;
        public MongoDbInitializer(string connection)
        {
            _connection = connection;
        }

        public void EnsureSeedData()
        {
            var context = new MongoDbContext(_connection);
            IMongoDatabase db = context.Database("mongocore");
            IMongoCollection<Skill> skillCollection = context.Collection<Skill>("mongocore", "skillCollection");

            var count = skillCollection.Count(new BsonDocument());
            if (count > 0)
            {
                Console.WriteLine($">>>>> {count} Skills already exist in collection");
                return;
            }

            skillCollection.InsertOne(new Skill(0, "Melee", ActionType.Attack, ElementType.Physical, EffectType.Damage, 0, 0, 2, false, 10, true, 2, 7));
            skillCollection.InsertOne(new Skill(1, "Shoot", ActionType.Attack, ElementType.Physical, EffectType.Damage, 0, 0, 18, false, 10, true, 2, 7));
            
            skillCollection.InsertOne(new Skill(10, "FireArrow", ActionType.Attack, ElementType.Fire, EffectType.Damage, 3, 0, 18, false, 10, true, 3, 11));
            skillCollection.InsertOne(new Skill(11, "Fireball", ActionType.Attack, ElementType.Fire, EffectType.Damage, 5, 0, 18, true, 3, true, 4, 8));
            skillCollection.InsertOne(new Skill(12, "FireStorm", ActionType.Attack, ElementType.Fire, EffectType.Damage, 10, 0, 18, true, 5, true, 5, 20));
            
            skillCollection.InsertOne(new Skill(20, "IceArrow", ActionType.Attack, ElementType.Water, EffectType.Damage, 3, 0, 18, false, 10, true, 3, 11));
            skillCollection.InsertOne(new Skill(21, "IceBall", ActionType.Attack, ElementType.Water, EffectType.Damage, 5, 0, 18, true, 3, true, 4, 8));
            skillCollection.InsertOne(new Skill(22, "Blizzard", ActionType.Attack, ElementType.Water, EffectType.Damage, 10, 0, 18, true, 5, true, 5, 20));
            
            skillCollection.InsertOne(new Skill(30, "Heal", ActionType.Heal, ElementType.Light, EffectType.Heal, 3, 0, 18, false, 10, true, 3, 7));
            skillCollection.InsertOne(new Skill(31, "Cure", ActionType.Heal, ElementType.Light, EffectType.Heal, 3, 0, 4, false, 10, true, 7, 11));
            skillCollection.InsertOne(new Skill(32, "GroupHeal", ActionType.Heal, ElementType.Light, EffectType.Heal, 5, 0, 18, true, 4, true, 4, 8));
            skillCollection.InsertOne(new Skill(33, "GreatHeal", ActionType.Heal, ElementType.Light, EffectType.Heal, 6, 0, 18, false, 10, true, 7, 17));

            count = skillCollection.Count(new BsonDocument());
            Console.WriteLine($">>>>> {count} documents created in collection");
        }
    }
}
