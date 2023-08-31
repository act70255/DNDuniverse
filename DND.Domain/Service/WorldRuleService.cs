using DND.Domain.Interface;
using DND.Domain.Service.Interface;
using DND.Model.Entity;
using DND.Repository;
using DND.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Service
{
    internal class WorldRuleService : IWorldRuleService
    {
        ITerrariaRepository _terrariaRepository;
        ISkillRepository _skillRepository;
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

            while (creature.Experience.CheckAdd(value))
            {
                value = creature.Experience.Add(value);
                creature.Level.Add(1);
                creature.Experience.MaxValue = Convert.ToInt32(Math.Pow(creature.Level.Value, 0.5)) * 20;
                creature.Experience.Value = 0;
            }
            return _terrariaRepository.UpdateCreature(creature);
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
                var baseDamage = RollEffect(skill);
                if (baseDamage >= target.ArmorClass.Value)
                {
                    var extraDamage = (source.Dexterity.Value > target.Dexterity.Value) ? Dice.Roll(sourceCreature.AttackBonus.Value) : 0;
                    var armor = target.ArmorClass.Value;
                    var hurt = baseDamage + extraDamage - armor;
                    Console.WriteLine($"{source.Name} attacks {target.Name} {target.Health.Value}/{target.Health.MaxValue} for {hurt}.");
                    if (hurt > 0)
                        target.Health.Sub(hurt);
                }
                //Terraria.Instance.Update(target);
                if (target.Health.Value <= 0)
                {
                    Console.WriteLine($"{target.Name} has died.");
                    AddExperience(source.ID, target.Level.Value);
                }
                return target;
            }

            Creature Heal(Skill skill, Creature source, Creature target)
            {
                var heal = RollEffect(skill);
                Console.WriteLine($"{source.Name} heals {target.Name} {target.Health.Value}/{target.Health.MaxValue} for {heal}.");
                target.Health.Add(heal);
                return target;
            }

            int RollEffect(Skill skill)
            {
                return Dice.Roll(skill.MaxValue, skill.Value);
            }
        }
    }
}
