using DND.Domain.Interface;
using DND.Domain.Service.Interface;
using DND.Model.Entity;
using DND.Repository;
using DND.Repository.Implement;
using DND.Repository.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Service
{
    internal class WorldRuleService : IWorldRuleService
    {
        private ITerrariaRepository _terrariaRepository;
        private ISkillRepository _skillRepository;

        public WorldRuleService(ISkillRepository skillRepository, ITerrariaRepository terrariaRepository)
        {
            _skillRepository = skillRepository;
            _terrariaRepository = terrariaRepository;
        }

        public Creature AddExperience(int sourceID, int value)
        {
            var creature = _terrariaRepository.GetByID(sourceID);
            if (creature == null)
                throw new Exception("Creature not found");

            while (creature.Experience.Value + value > creature.Experience.MaxValue)
            {
                value = value - creature.Experience.MaxValue + creature.Experience.Value;
                creature.Level.Add(1);
                creature.Experience.MaxValue = Convert.ToInt32(Math.Pow(creature.Level.Value, 0.5)) * 20;
                creature.Experience.Value = 0;
            }
            creature.Experience.Value += value;

            var filter = Builders<Creature>.Filter.Eq("ID", creature.ID);
            var update = Builders<Creature>.Update.Set("Experience", creature.Experience)
                                                  .Set("Level", creature.Level);
            var result = _terrariaRepository.UpdateCreature(filter, update);
            return creature;
        }

        public Creature Spell(int skillID, int sourceID, int targetID)
        {
            var sourceCreature = _terrariaRepository.GetByID(sourceID);
            if (sourceCreature == null)
                throw new Exception("Creature not found.");
            var targetCreature = _terrariaRepository.GetByID(targetID);
            if (targetCreature == null)
                throw new Exception("Creature not found.");
            if (!sourceCreature.Skills.Contains(skillID))
                throw new Exception("Creature does not know this skill.");
            var effectSkill = _skillRepository.Get(skillID);
            if (effectSkill == null)
                throw new Exception("Skill not found.");

            return Spell(effectSkill, sourceCreature, targetCreature);

            Creature Spell(Skill skill, Creature source, Creature target)
            {
                switch (skill.EffectType)
                {
                    case EffectType.Damage:
                        return Attack(skill, source, target);
                    case EffectType.Heal:
                        return Heal(skill, source, target);
                    default:
                        return target;
                }
            }

            Creature Attack(Skill skill, Creature source, Creature target)
            {
                int gainExp = 1;
                var baseDamage = RollEffect(skill);
                if (baseDamage >= target.ArmorClass.Value)
                {
                    var extraDamage = (source.Dexterity.Value > target.Dexterity.Value) ? Dice.Roll(sourceCreature.AttackBonus.Value) : 0;
                    var armor = target.ArmorClass.Value;
                    var hurt = baseDamage + extraDamage - armor;
                    Console.WriteLine($"{source.Name} attacks {target.Name} {target.Health.Value}/{target.Health.MaxValue} for {hurt}.");
                    if (hurt > 0)
                    {
                        target.Health.Sub(hurt);
                        gainExp += hurt / 10;
                    }
                }

                if (target.Health.Value <= 0)
                {
                    gainExp += target.Level.Value;
                    Console.WriteLine($"{target.Name} has died.");
                    //AddExperience(source.ID, target.Level.Value);
                }
                var filter = Builders<Creature>.Filter.Eq("ID", target.ID);
                var update = Builders<Creature>.Update.Set("Health", target.Health);
                _terrariaRepository.UpdateCreature(filter, update);

                AddExperience(source.ID, gainExp);
                return target;
            }

            Creature Heal(Skill skill, Creature source, Creature target)
            {
                int gainExp = 1;
                var heal = RollEffect(skill);
                Console.WriteLine($"{source.Name} heals {target.Name} {target.Health.Value}/{target.Health.MaxValue} for {heal}.");
                target.Health.Add(heal);
                gainExp+= heal / 10;
                AddExperience(source.ID, gainExp);
                return target;
            }

            int RollEffect(Skill skill)
            {
                return Dice.Roll(skill.MaxValue, skill.Value);
            }
        }
    }
}
