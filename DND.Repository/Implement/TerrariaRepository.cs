using AutoMapper.Features;
using DND.Model.Entity;
using DND.Repository.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Repository.Implement
{
    public class TerrariaRepository : Repository<Skill>, ITerrariaRepository
    {
        IMongoCollection<Creature> _creatureCollection;
        IMongoDatabase _mongoDB;
        Stopwatch _stopwatch;
        public TerrariaRepository(IMongoDatabase mongoDB) : base(mongoDB)
        {
            _mongoDB = mongoDB;
            _creatureCollection = _mongoDB.GetCollection<Creature>(typeof(Creature).Name);
        }
        public Creature NewCreature(string name, int health = 10, int mana = 10, int stamina = 10, int experience = 20, int level = 10, int armorClass = 10, int attackBonus = 10, int damage = 10, int strength = 10, int dexterity = 10, int intelligence = 10, int charisma = 10)
        {
            var newID = _creatureCollection.Find(f => true).ToEnumerable().Max(m => m.ID) + 1;
            var creature = new Creature(newID, name, health, mana, stamina, experience, level, armorClass, attackBonus, damage, strength, dexterity, intelligence, charisma);
            _creatureCollection.InsertOne(creature);
            return creature;
        }
        public Creature NewCreature(Creature creature)
        {
            creature.ID = _creatureCollection.Find(f => true).Any() ? (_creatureCollection.Find(f => true).ToEnumerable().Max(m => m.ID) + 1) : 0;
            _creatureCollection.InsertOne(creature);
            return creature;
        }

        public IEnumerable<Creature> GetAllCreatures()
        {
            return _creatureCollection.Find(f => true).ToEnumerable();
        }

        public Creature GetByID(int id)
        {
            return _creatureCollection.Find(f => f.ID == id).FirstOrDefault();
        }

        public UpdateResult UpdateCreature(FilterDefinition<Creature> filterDefinition,UpdateDefinition<Creature> updateDefinition)
        {
            _stopwatch.Restart();
            var result = _creatureCollection.UpdateOne(filterDefinition,updateDefinition);
            Debug.WriteLine($"UpdateCreature: {_stopwatch.ElapsedMilliseconds}ms");
            return result;
        }
    }
}
