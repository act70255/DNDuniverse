using AutoMapper;
using DND.Domain.Interface;
using DND.Domain.Service.Interface;
using DND.Model.ApiContent;
using DND.Model.Entity;
using DND.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DND.Domain.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DNDController : ControllerBase
    {
        ILogger<DNDController> _logger;
        ITerrariaService _terrariaService;
        ICreatureService _creatureService;
        IWorldRuleService _worldRuleService;
        IWorldElementService _worldElementService;
        IMapper _mapper;

        public DNDController(ILogger<DNDController> logger, ITerrariaService terrariaService, ICreatureService creatureService, IWorldRuleService worldRuleService, IWorldElementService worldElementService, IMapper mapper)
        {
            _logger = logger;
            _terrariaService = terrariaService;
            _creatureService = creatureService;
            _worldRuleService = worldRuleService;
            _worldElementService = worldElementService;
            _mapper = mapper;
        }

        [HttpPost(Name = "~/Data")]
        public IActionResult Data([FromBody] dynamic queyr)
        {
            return Ok(DateTime.Now.ToString());
        }

        [HttpPost(Name = "~/GetCreatures")]
        public ActionResult<IEnumerable<CreatureRequest>> GetCreatures()
        {
            try
            {
                //_logger.LogInformation("GetCreatures");
                var resultData = _terrariaService.GetCreatures();
                return resultData == null ? NotFound() : Ok(_mapper.Map<IEnumerable<CreatureRequest>>(resultData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost(Name = "~/NewCreatures")]
        public ActionResult<Creature> NewCreatures(CreatureRequest request)
        {
            try
            {
                return Ok(_terrariaService.AddCreature(_mapper.Map<Creature>(request)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost(Name = "~/Action")]
        public ActionResult<CreatureRequest> Action(ActionRequest request)
        {
            try
            {
                var result = _worldRuleService.Spell(request.Skill, request.Source, request.Target);
                var response = _mapper.Map<CreatureRequest>(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost(Name = "~/GetSkills")]
        public ActionResult<IEnumerable<Skill>> GetSkills(SkillsRequest request)
        {
            try
            {
                if (request != null && request.IDs.Any())
                    return Ok(_terrariaService.GetSkills().Where(x => request.IDs.Contains(x.ID)));
                else
                    return Ok(_terrariaService.GetSkills());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost(Name = "~/AddSKills")]
        public ActionResult<Creature> AddSKills(SkillsRequest request)
        {
            try
            {
                var result = _creatureService.AddSkills(request.ID, request.IDs.ToArray());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
