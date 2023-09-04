using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using DND.Domain.Service.Interface;
using DND.Repository;
using DND.Model.Entity;
using DND.Repository.Interface;
using MongoDB.Driver;

namespace DND.Domain.Service
{
    internal class CreatureService : ICreatureService
    {
        private ISkillRepository _skillRepository;
        private ITerrariaRepository _terrariaRepository;
        public CreatureService(ISkillRepository skillRepository, ITerrariaRepository terrariaRepository)
        {
            _skillRepository = skillRepository;
            _terrariaRepository = terrariaRepository;
        }

        public Creature AddSkills(int creatureID, int[] skillIDs)
        {
            var creature = _terrariaRepository.GetByID(creatureID);
            if (creature == null)
                throw new Exception("Creature not found.");
            var skills = _skillRepository.GetAll().Select(s => s.ID).Where(f => skillIDs.Contains(f));
            if (skills == null || !skills.Any())
                throw new Exception("Skill not found.");

            var newSkills = skills.Where(a => !creature.Skills.Contains(a));

            creature.Skills = creature.Skills.Concat(newSkills).Distinct().OrderBy(o => o).ToArray();

            var filter = Builders<Creature>.Filter.Eq("ID", creature.ID);
            var update = Builders<Creature>.Update.Set("Health", creature.Health);
            var result = _terrariaRepository.UpdateCreature(filter, update);
            return creature;
        }
    }
}
