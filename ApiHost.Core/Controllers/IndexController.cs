using AutoMapper;
using AutoMapper.Configuration;
using DND.Domain.Interface;
using DND.Domain.Service.Interface;
using DND.Model.ApiContent;
using DND.Model.Entity;
using DND.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;

namespace ApiHost.Core.Controller
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndexController : ControllerBase
    {
        private ILogger<IndexController> _logger;
        private IMapper _mapper;

        public IndexController(ILogger<IndexController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost(Name = "Data")]
        public IActionResult Data(object data)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            return Ok(DateTime.Now);
        }

        [HttpPost]
        public IActionResult Sysinfo()
        {
            var result = DateTime.Now;
            return Ok(result);
        }
    }
}
