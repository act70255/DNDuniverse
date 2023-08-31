using DND.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Repository.Interface
{
    public interface ITerrariaRepository
    {
        Creature NewCreature(string name, int health = 10, int mana = 10, int stamina = 10, int experience = 20, int level = 10, int armorClass = 10, int attackBonus = 10, int damage = 10, int strength = 10, int dexterity = 10, int intelligence = 10, int charisma = 10);
        Creature NewCreature(Creature creature);
        IEnumerable<Creature> GetAllCreatures();
        Creature GetByID(int id);
        Creature UpdateCreature(Creature creature);
    }
}
