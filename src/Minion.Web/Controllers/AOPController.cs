using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minion.Web.Domain;
using Minion.Web.Presentation;

namespace Minion.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AOPController : Controller
    {
        private readonly IPresentationLogic _logic;

        public AOPController(IPresentationLogic logic)
        {
            _logic = logic;
        }

        [HttpGet]
        public string Validation()
        {
            return _logic.Validate();
        }

        [HttpGet]
        public string Standardization()
        {
            return _logic.Standardize();
        }

        [HttpGet]
        public string Transformation()
        {
            return _logic.Transform();
        }

        [HttpGet]
        public string Conditional()
        {
            return _logic.Condition();
        }

        [HttpGet]
        public string Auditing()
        {
            return _logic.Audit();
        }

        [HttpGet]
        public string EventSourcing()
        {
            return _logic.EventSource();
        }
    }
}
