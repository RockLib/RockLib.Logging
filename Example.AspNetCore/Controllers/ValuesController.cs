using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RockLib.Logging;

namespace Example.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger _logger;

        public ValuesController(ILogger logger)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.Info("GET api/values");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            _logger.Info($"GET api/values/{id}");
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            _logger.Info("POST api/values", new { value });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            _logger.Info($"PUT api/values/{id}", new { value });
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.Info($"DELETE api/values/{id}");
        }
    }
}
