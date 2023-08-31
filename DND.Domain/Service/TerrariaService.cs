using DND.Domain.Service.Interface;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DND.Repository;
using DND.Model.Entity;
using DND.Repository.Interface;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace DND.Domain.Service
{
    internal class TerrariaService : ITerrariaService
    {
        IMapper _mapper;
        ICreatureService _creatureService;
        ISkillRepository _skillRepository;
        ITerrariaRepository _terrariaRepository;
        IConfiguration _configuration;
        public TerrariaService(ICreatureService creatureService, IMapper mapper, IConfiguration configuration, ISkillRepository skillRepository, ITerrariaRepository terrariaRepository)
        {
            _skillRepository = skillRepository;
            _configuration = configuration;
            _creatureService = creatureService;
            _mapper = mapper;
            _terrariaRepository = terrariaRepository;
        }

        public IEnumerable<Creature> GetCreatures()
        {
            return _terrariaRepository.GetAllCreatures();
        }

        public Creature AddCreature(Creature request)
        {
            return _terrariaRepository.NewCreature(request);
        }

        public IEnumerable<Skill> GetSkills()
        {
            return _skillRepository.GetAll();
        }
    }
}
