using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minion.Web.Models;

namespace Minion.Web.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger _log;
        private readonly ICoreConfiguration _config;
        private readonly Settings _settings;
        private readonly IBusinessLogic _logic;
        private readonly ITest _test;

        public ValuesController(ILoggerFactory log, ICoreConfiguration config, Settings settings, IBusinessLogic logic, ITest test)
        {
            _log = log.CreateLogger<ValuesController>();
            _config = config;
            _settings = settings;
            _logic = logic;
            _test = test;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var ids = _logic.GetId() + " | " + _test.Id.ToString();
            return new string[] { "value1", "value2", ids };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
